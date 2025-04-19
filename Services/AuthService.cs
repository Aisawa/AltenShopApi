using AltenShopApi.Data;
using AltenShopApi.DTO;
using AltenShopApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AltenShopApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly AltenShopDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(AltenShopDbContext context, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ApiResponse<User>> RegisterAsync(RegisterUserDto model)
        {
            try
            {
                // Vérifier si l'email existe déjà
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    return ApiResponse<User>.CreateError(
                        HttpStatusCode.BadRequest,
                        "Email already exists");
                }

                // Vérifier si le nom d'utilisateur existe déjà
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    return ApiResponse<User>.CreateError(
                        HttpStatusCode.BadRequest,
                        "Username already exists");
                }

                // On hache le mot de passe
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                // On créer un nouvel utilisateur
                var user = new User
                {
                    Username = model.Username,
                    Firstname = model.Firstname,
                    Email = model.Email,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow
                };

                // On ajoute l'utilisateur à la base de données
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return ApiResponse<User>.CreateSuccess(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering new user");
                return ApiResponse<User>.CreateError(
                    HttpStatusCode.InternalServerError,
                    "Failed to register user");
            }
        }

		public async Task<ApiResponse<TokenResponse>> LoginAsync(LoginDto model)
		{
			try
			{
				var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
				if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
				{
					return ApiResponse<TokenResponse>.CreateError(
						HttpStatusCode.Unauthorized,
						"Invalid email or password");
				}

				var tokenResponse = GenerateJwtToken(user);
				return ApiResponse<TokenResponse>.CreateSuccess(tokenResponse);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login");
				return ApiResponse<TokenResponse>.CreateError(
					HttpStatusCode.InternalServerError,
					"An error occurred during login");
			}
		}

		private TokenResponse GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"];
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];

            if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
            {
                throw new InvalidOperationException("JWT configuration is missing");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("username", user.Username)
        };

            var expiration = DateTime.UtcNow.AddHours(3);
            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return new TokenResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration,
                Username = user.Username,
				Email = user.Email
			};
        }
    }
}

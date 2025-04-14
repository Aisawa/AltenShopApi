using AltenShopApi.Data;
using AltenShopApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development,
    // Force explicitement les URLs HTTP
    //WebHostDefaults = {
    //    UseUrls = "http://localhost:5186"
    //}
});

builder.Services.Configure<HttpsRedirectionOptions>(options =>
{
    options.HttpsPort = null;
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenLocalhost(5186); // HTTP uniquement
});

builder.WebHost.UseUrls("http://localhost:5186");

// Configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container
builder.Services.AddAuthorization(); // <-- Correction ici
builder.Services.AddControllers();   // Nécessaire pour les API controllers

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
// Configurer swagger pour tester JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AltenShop API", Version = "v1" });

    // Configuration pour JWT dans Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();

// Configure caching
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configuration Entity Framework avec SQLite (ou SQL Server selon votre préférence)
builder.Services.AddDbContext<AltenShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enregistrer le service de base de données des séries
builder.Services.AddScoped<IProductService, ProductService>();

// Configurer JWT
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});


var app = builder.Build();

app.Use(async (context, next) =>
{
    // Intercepte les tentatives de redirection vers HTTPS
    if (context.Request.IsHttps || context.Request.Host.Port == 7209)
    {
        var newUrl = new UriBuilder(context.Request.Scheme, "localhost", 5186)
        {
            Path = context.Request.Path,
            Query = context.Request.QueryString.Value
        }.Uri.ToString();

        context.Response.Redirect(newUrl, false);
        return;
    }
    await next();
});

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
        options.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer>
            {
                new OpenApiServer { Url = $"http://{httpReq.Host}" }
            };
        });
    });

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.ConfigObject.AdditionalItems["urls.primaryName"] = "HTTP";
    });
    app.UseDeveloperExceptionPage();
}

//app.Use(async (context, next) =>
//{
//    context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:4200");
//    await next();
//});
app.UseRouting();
app.UseCors("AllowAngular");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // Gardez la redirection HTTPS seulement en production
}
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

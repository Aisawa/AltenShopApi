using AltenShopApi.DTO;
using AltenShopApi.Models;

namespace AltenShopApi.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<User>> RegisterAsync(RegisterUserDto model);
        Task<ApiResponse<TokenResponse>> LoginAsync(LoginDto model);
    }
}

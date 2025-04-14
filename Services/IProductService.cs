using AltenShopApi.Models;
using AltenShopApi.Models.Database;

namespace AltenShopApi.Services
{
    public interface IProductService
    {
        Task<ApiResponse<List<DbProduct>>> GetAllProductsAsync();
        Task<ApiResponse<DbProduct>> GetProductByIdAsync(string id);
        Task<ApiResponse<bool>> DeleteProductAsync(string id);
        Task<ApiResponse<DbProduct>> UpdateProductAsync(string id, DbProduct product);
        Task<ApiResponse<DbProduct>> CreateProductAsync(DbProduct product);
    }
}

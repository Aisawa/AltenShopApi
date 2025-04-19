using AltenShopApi.Data;
using AltenShopApi.Models;
using AltenShopApi.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AltenShopApi.Services
{
    public class ProductService : IProductService
    {

        private readonly AltenShopDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AltenShopDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApiResponse<List<DbProduct>>> GetAllProductsAsync()
        {
            try
            {
                var product = await _context.Products
                    .ToListAsync();

                return ApiResponse<List<DbProduct>>.CreateSuccess(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products from database");
                return ApiResponse<List<DbProduct>>.CreateError(
                    HttpStatusCode.InternalServerError,
                    "Failed to retrieve product from database");
            }
        }

        public async Task<ApiResponse<DbProduct>> GetProductByIdAsync(string id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(s => s.Id == Convert.ToInt32(id));

                if (product == null)
                {
                    return ApiResponse<DbProduct>.CreateError(
                        HttpStatusCode.NotFound,
                        $"Product with ID {id} not found");
                }

                return ApiResponse<DbProduct>.CreateSuccess(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product {ProductId} from database", id);
                return ApiResponse<DbProduct>.CreateError(
                    HttpStatusCode.InternalServerError,
                    "Failed to retrieve product from database");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProductAsync(string id)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(s => s.Id == Convert.ToInt32(id));

                if (product == null)
                {
                    return ApiResponse<bool>.CreateError(
                        HttpStatusCode.NotFound,
                        $"Product with ID {id} not found");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} deleted successfully", id);
                return ApiResponse<bool>.CreateSuccess(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId} from database", id);
                return ApiResponse<bool>.CreateError(
                    HttpStatusCode.InternalServerError,
                    "Failed to delete product from database");
            }
        }

        public async Task<ApiResponse<DbProduct>> UpdateProductAsync(string id, DbProduct product)
        {
            try
            {
                int productId = Convert.ToInt32(id);
                if (productId != product.Id)
                {
                    return ApiResponse<DbProduct>.CreateError(
                        HttpStatusCode.BadRequest,
                        "ID in URL does not match ID in request body");
                }

                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(s => s.Id == productId);

                if (existingProduct == null)
                {
                    return ApiResponse<DbProduct>.CreateError(
                        HttpStatusCode.NotFound,
                        $"Product with ID {id} not found");
                }

                // Mettre à jour les propriétés
                existingProduct.Code = product.Code;
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Image = product.Image;
                existingProduct.Category = product.Category;
                existingProduct.Price = product.Price;
                existingProduct.InternalReference = product.InternalReference;
                existingProduct.ShellId = product.ShellId;
                existingProduct.InventoryStatus = product.InventoryStatus;
                existingProduct.Rating = product.Rating;
                existingProduct.UpdatedAt = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _context.Products.Update(existingProduct);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductId} updated successfully", id);
                return ApiResponse<DbProduct>.CreateSuccess(existingProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId} in database", id);
                return ApiResponse<DbProduct>.CreateError(
                    HttpStatusCode.InternalServerError,
                    "Failed to update product in database");
            }
        }

        public async Task<ApiResponse<DbProduct>> CreateProductAsync(DbProduct product)
        {
            try
            {
                if (string.IsNullOrEmpty(product.Name))
                {
                    return ApiResponse<DbProduct>.CreateError(
                        HttpStatusCode.BadRequest,
                        "Product name is required");
                }

                // On ajoute le produit
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Product {ProductName} created successfully with ID {ProductId}",
                    product.Name, product.Id);

                return ApiResponse<DbProduct>.CreateSuccess(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product in database");
                return ApiResponse<DbProduct>.CreateError(
                    HttpStatusCode.InternalServerError,
                    "Failed to create product in database");
            }
        }
    }
}

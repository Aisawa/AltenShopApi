using AltenShopApi.Attributes;
using AltenShopApi.Models.Database;
using AltenShopApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AltenShopApi.Controllers
{
    public class ProductController : Controller
    {

        private readonly IProductService _productsService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productsService, ILogger<ProductController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProducts()
        {
            _logger.LogInformation("Getting all products from database");
            var response = await _productsService.GetAllProductsAsync();

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response);
        }

        [HttpGet("products/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetProductById(string id)
        {
            _logger.LogInformation("Getting product by id from database");
            var response = await _productsService.GetProductByIdAsync(id);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return Ok(response);
        }

        [HttpDelete("products/{id}")]
        //[AdminOnly]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            _logger.LogInformation("Deleting product with id {ProductId}", id);
            var response = await _productsService.DeleteProductAsync(id);
            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("products/{id}")]
        //[AdminOnly]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] DbProduct product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Updating product with id {ProductId}", id);
            var response = await _productsService.UpdateProductAsync(id, product);
            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("products")]
        //[AdminOnly]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromBody] DbProduct product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Creating new product");
            var response = await _productsService.CreateProductAsync(product);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response);
            }

            return CreatedAtAction(nameof(GetProductById), new { id = response.Data.Id }, response);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ecommerce.Service.Product.BusinessLayer;
using Ecommerce.Service.Product.BusinessLayer.Dtos;
using System;

namespace Ecommerce.Service.Product.WebAPI
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductManager productManager, ILogger<ProductController> logger)
        {
            _productManager = productManager;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                _logger.LogInformation("Fetching product with ID: {ProductId}", id);
                var product = await _productManager.GetProductAsync(id);
                if (product == null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found", id);
                    return NotFound(new { Message = "Product not found" });
                }
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Bad request for product {ProductId}: {Message}", id, ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        {
            try
            {
                _logger.LogInformation("Creating product: {ProductName}", product?.ProductName);
                var result = await _productManager.CreateProductAsync(product);
                return Ok(new { Message = "Product created successfully", Result = result });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Failed to create product: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto product)
        {
            try
            {
                _logger.LogInformation("Updating product ID: {ProductId}", product?.Id);
                var result = await _productManager.UpdateProductAsync(product);
                if (!result)
                {
                    _logger.LogWarning("Product ID {ProductId} not found or update failed", product?.Id);
                    return NotFound(new { Message = "Product not found or update failed" });
                }
                return Ok(new { Message = "Product updated successfully" });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Bad request updating product: {Message}", ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _logger.LogInformation("Deleting product ID: {ProductId}", id);
                var result = await _productManager.DeleteProductAsync(id);
                if (!result)
                {
                    _logger.LogWarning("Product ID {ProductId} could not be deleted", id);
                    return NotFound(new { Message = "Product not found or could not be deleted" });
                }
                return Ok(new { Message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product ID: {ProductId}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] int count = 50)
        {
            _logger.LogInformation("Fetching latest {Count} audit logs", count);
            var logs = await _productManager.GetLogsAsync(count);
            return Ok(logs);
        }

        [HttpGet("logs/count")]
        public async Task<IActionResult> GetLogsCount()
        {
            var count = await _productManager.GetLogsCountAsync();
            return Ok(new { LogCount = count });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _productManager.GetCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("categories/{categoryId}/subcategories")]
        public async Task<IActionResult> GetSubCategories(int categoryId)
        {
            var subCategories = await _productManager.GetSubCategoriesAsync(categoryId);
            return Ok(subCategories);
        }

        [HttpGet("subcategories")]
        public async Task<IActionResult> GetAllSubCategories()
        {
            var subCategories = await _productManager.GetAllSubCategoriesAsync();
            return Ok(subCategories);
        }

        [HttpGet("genders")]
        public async Task<IActionResult> GetGenders()
        {
            var genders = await _productManager.GetGendersAsync();
            return Ok(genders);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromQuery] int? categoryId, [FromQuery] int? subCategoryId, [FromQuery] int? genderId)
        {
            var products = await _productManager.GetProductsAsync(categoryId, subCategoryId, genderId);
            return Ok(products);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertProduct([FromBody] ProductDto product)
        {
            try
            {
                _logger.LogInformation("Upserting product: {ProductName} (ID: {ProductId})", product?.ProductName, product?.Id);
                var message = await _productManager.UpsertProductAsync(product);
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upserting product: {Message}", ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}

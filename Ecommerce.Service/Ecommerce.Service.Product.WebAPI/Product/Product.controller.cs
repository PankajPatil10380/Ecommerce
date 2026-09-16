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

        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            try
            {
                var product = await _productManager.GetProductAsync(id);
                if (product == null) return NotFound(new { Message = "Product not found" });
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto product)
        {
            try
            {
                var result = await _productManager.CreateProductAsync(product);
                return Ok(new { Message = "Product created successfully", Result = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDto product)
        {
            try
            {
                var result = await _productManager.UpdateProductAsync(product);
                if (!result) return NotFound(new { Message = "Product not found or update failed" });
                return Ok(new { Message = "Product updated successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var result = await _productManager.DeleteProductAsync(id);
                if (!result) return NotFound(new { Message = "Product not found or could not be deleted" });
                return Ok(new { Message = "Product deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
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
                var message = await _productManager.UpsertProductAsync(product);
                return Ok(new { Message = message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}

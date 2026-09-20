using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Ecommerce.UI.ViewModel;
using Ecommerce.UI.ServiceLayer;
using System.Collections.Generic;

namespace Ecommerce.UI.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ProductServiceClient _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ProductServiceClient productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? categoryId, int? subCategoryId, int? genderId)
        {
            _logger.LogInformation("Products Index Action called with CategoryId={CategoryId}, SubCategoryId={SubCategoryId}, GenderId={GenderId}", categoryId, subCategoryId, genderId);
            var model = new ProductViewModel
            {
                Categories = await _productService.GetCategoriesAsync(),
                Genders = await _productService.GetGendersAsync(),
                SelectedCategoryId = categoryId,
                SelectedSubCategoryId = subCategoryId,
                SelectedGenderId = genderId
            };

            if (categoryId.HasValue)
            {
                model.SubCategories = await _productService.GetSubCategoriesAsync(categoryId.Value);
            }
            else
            {
                // Load all sub-categories so the dashboard can show names instead of IDs
                model.SubCategories = await _productService.GetAllSubCategoriesAsync();
            }

            model.FilteredProducts = await _productService.GetProductsAsync(categoryId, subCategoryId, genderId);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var model = new ProductViewModel
                {
                    Categories = await _productService.GetCategoriesAsync(),
                    Genders = await _productService.GetGendersAsync()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error loading Create page: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] ProductDto product)
        {
            if (product == null)
            {
                return Json(new { success = false, message = "Product data is missing." });
            }

            try
            {
                var success = await _productService.CreateProductAsync(product);
                if (success)
                {
                    return Json(new { success = true, message = "Product created successfully!" });
                }
                return Json(new { success = false, message = "Failed to create product." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null) return NotFound();

                var model = new ProductViewModel
                {
                    Product = product,
                    Categories = await _productService.GetCategoriesAsync(),
                    Genders = await _productService.GetGendersAsync()
                };

                if (product.SubCategoryId != 0)
                {
                    model.SubCategories = await _productService.GetSubCategoriesAsync(product.SubCategoryId);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error loading Edit page: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAjax([FromBody] ProductDto product)
        {
            if (product == null)
            {
                return Json(new { success = false, message = "Product data is missing." });
            }

            try
            {
                var success = await _productService.UpdateProductAsync(product);
                if (success)
                {
                    return Json(new { success = true, message = "Product updated successfully!" });
                }
                return Json(new { success = false, message = "Failed to update product." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (success)
            {
                return Json(new { success = true, message = "Product deleted successfully!" });
            }
            return Json(new { success = false, message = "Failed to delete product. It might be linked to existing orders." });
        }
    }
}

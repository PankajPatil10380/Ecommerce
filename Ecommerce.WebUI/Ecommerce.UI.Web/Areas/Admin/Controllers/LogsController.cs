using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Ecommerce.UI.ServiceLayer;

namespace Ecommerce.UI.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LogsController : Controller
    {
        private readonly ProductServiceClient _productService;
        private readonly ILogger<LogsController> _logger;

        public LogsController(ProductServiceClient productService, ILogger<LogsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int count = 50)
        {
            _logger.LogInformation("Loading Audit Logs page, count: {Count}", count);
            var logs = await _productService.GetLogsAsync(count);
            ViewData["Count"] = count;
            return View(logs);
        }
    }
}

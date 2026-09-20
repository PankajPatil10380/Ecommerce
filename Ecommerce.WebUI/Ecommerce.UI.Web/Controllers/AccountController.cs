using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecommerce.UI.ServiceLayer;
using Ecommerce.UI.ViewModel;

namespace Ecommerce.UI.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ProductServiceClient _productService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ProductServiceClient productService, ILogger<AccountController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Processing login for user {Username}", model.Username);
            var result = await _productService.LoginAsync(model);

            if (!result.IsSuccess || string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Username ?? model.Username),
                new Claim(ClaimTypes.Role, result.Role ?? "User"),
                new Claim("JWT_TOKEN", result.Token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = result.ExpiresAt ?? DateTime.UtcNow.AddHours(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
            HttpContext.Session.SetString("JWToken", result.Token);

            _logger.LogInformation("User {Username} successfully logged into WebUI with role {Role}", model.Username, result.Role);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (string.Equals(result.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Products", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            _logger.LogInformation("Processing registration for user {Username}", model.Username);
            var result = await _productService.RegisterAsync(model);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Registration successful! Please log in with your credentials.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

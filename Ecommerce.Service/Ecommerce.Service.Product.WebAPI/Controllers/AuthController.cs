using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ecommerce.Service.Product.BusinessLayer.Auth;
using Ecommerce.Service.Product.BusinessLayer.Dtos;

namespace Ecommerce.Service.Product.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Login request received for user {Username}", loginDto?.Username);
                var response = await _authManager.LoginAsync(loginDto!);

                if (!response.IsSuccess)
                {
                    return Unauthorized(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during login for user {Username}", loginDto?.Username);
                return StatusCode(500, new AuthResponseDto { IsSuccess = false, Message = "An unexpected error occurred during login." });
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _logger.LogInformation("Registration request received for user {Username}", registerDto?.Username);
                var response = await _authManager.RegisterAsync(registerDto!);

                if (!response.IsSuccess)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during registration for user {Username}", registerDto?.Username);
                return StatusCode(500, new AuthResponseDto { IsSuccess = false, Message = "An unexpected error occurred during registration." });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var username = User.Identity?.Name ?? User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { Message = "User identity not found in token." });
                }

                var user = await _authManager.GetUserByUsernameAsync(username);
                if (user == null)
                {
                    return NotFound(new { Message = "User profile not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception retrieving profile");
                return StatusCode(500, new { Message = "An unexpected error occurred." });
            }
        }
    }
}

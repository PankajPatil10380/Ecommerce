using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ecommerce.Service.Product.DataLayer.Repository;
using Ecommerce.Service.Product.Domain.Entities;
using Ecommerce.Service.Product.BusinessLayer.Dtos;
using Ecommerce.Service.Product.BusinessLayer.Security;
using Ecommerce.Common.Services;

namespace Ecommerce.Service.Product.BusinessLayer.Auth
{
    public class AuthManager : IAuthManager
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILoggingService _loggingService;
        private readonly ILogger<AuthManager> _logger;

        public AuthManager(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            ILoggingService loggingService,
            ILogger<AuthManager> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _loggingService = loggingService;
            _logger = logger;
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Username and password are required." };
            }

            _logger.LogInformation("Attempting login for user: {Username}", loginDto.Username);

            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null)
            {
                _logger.LogWarning("Login failed. User not found: {Username}", loginDto.Username);
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed. User is inactive: {Username}", loginDto.Username);
                return new AuthResponseDto { IsSuccess = false, Message = "Account is inactive. Please contact administrator." };
            }

            bool isPasswordValid = _passwordHasher.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed. Invalid password for user: {Username}", loginDto.Username);
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid username or password." };
            }

            var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

            await _loggingService.LogAsync(
                "Information",
                "UserLogin",
                $"User '{user.Username}' logged in successfully.",
                "Users"
            );

            _logger.LogInformation("User {Username} logged in successfully.", user.Username);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                Username = user.Username,
                Role = user.Role,
                ExpiresAt = expiresAt,
                Message = "Login successful."
            };
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Registration details are required." };
            }

            _logger.LogInformation("Attempting registration for username: {Username}, email: {Email}", registerDto.Username, registerDto.Email);

            bool exists = await _userRepository.UserExistsAsync(registerDto.Username, registerDto.Email);
            if (exists)
            {
                _logger.LogWarning("Registration failed. Username or email already exists: {Username}, {Email}", registerDto.Username, registerDto.Email);
                return new AuthResponseDto { IsSuccess = false, Message = "Username or email is already registered." };
            }

            _passwordHasher.CreatePasswordHash(registerDto.Password, out string passwordHash, out string passwordSalt);

            var user = new User
            {
                Username = registerDto.Username.Trim(),
                Email = registerDto.Email.Trim().ToLowerInvariant(),
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = string.IsNullOrWhiteSpace(registerDto.Role) ? "User" : registerDto.Role.Trim(),
                FullName = registerDto.FullName?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateUserAsync(user);

            var (token, expiresAt) = _jwtTokenService.GenerateToken(user);

            await _loggingService.LogAsync(
                "Information",
                "UserRegister",
                $"New user '{user.Username}' registered with role '{user.Role}'.",
                "Users"
            );

            _logger.LogInformation("User {Username} registered successfully with ID {UserId}.", user.Username, user.Id);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Token = token,
                Username = user.Username,
                Role = user.Role,
                ExpiresAt = expiresAt,
                Message = "Registration successful."
            };
        }

        public async Task<UserDto?> GetUserByUsernameAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }
    }
}

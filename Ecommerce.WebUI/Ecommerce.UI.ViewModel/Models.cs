using System;
using System.Collections.Generic;

namespace Ecommerce.UI.ViewModel
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int GenderId { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductCategory
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    public class SubProductCategory
    {
        public int Id { get; set; }
        public string SubCategory { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }

    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class LogDto
    {
        public int Id { get; set; }
        public string LogLevel { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? TableName { get; set; }
        public int? RecordId { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class LoginViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Role { get; set; } = "User";
    }

    public class AuthResultDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Message { get; set; }
    }
}

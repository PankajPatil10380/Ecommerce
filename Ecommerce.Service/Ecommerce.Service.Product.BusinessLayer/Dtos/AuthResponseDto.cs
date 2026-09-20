using System;

namespace Ecommerce.Service.Product.BusinessLayer.Dtos
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Message { get; set; }
    }
}

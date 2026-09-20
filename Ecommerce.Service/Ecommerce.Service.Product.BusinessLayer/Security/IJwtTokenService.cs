using System;
using Ecommerce.Service.Product.Domain.Entities;

namespace Ecommerce.Service.Product.BusinessLayer.Security
{
    public interface IJwtTokenService
    {
        (string Token, DateTime ExpiresAt) GenerateToken(User user);
    }
}

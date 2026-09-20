using System.Threading.Tasks;
using Ecommerce.Service.Product.BusinessLayer.Dtos;

namespace Ecommerce.Service.Product.BusinessLayer.Auth
{
    public interface IAuthManager
    {
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<UserDto?> GetUserByUsernameAsync(string username);
    }
}

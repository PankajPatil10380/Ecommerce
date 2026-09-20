using System.Threading.Tasks;
using Ecommerce.Service.Product.Domain.Entities;

namespace Ecommerce.Service.Product.DataLayer.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<bool> UserExistsAsync(string username, string email);
        Task<int> CreateUserAsync(User user);
    }
}

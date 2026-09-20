using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Service.Product.Domain.Entities;
using Ecommerce.Service.Product.DataLayer;

namespace Ecommerce.Service.Product.DataLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ProductDbContext _context;

        public UserRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users.AnyAsync(u => 
                u.Username.ToLower() == username.ToLower() || 
                u.Email.ToLower() == email.ToLower());
        }

        public async Task<int> CreateUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user.Id;
        }
    }
}

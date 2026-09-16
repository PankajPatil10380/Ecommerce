using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Common.Entities;
using Ecommerce.Common.Interfaces;
using Ecommerce.Service.Product.DataLayer;

namespace Ecommerce.Service.Product.DataLayer.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly ProductDbContext _context;

        public LogRepository(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<bool> InsertLogAsync(Log log)
        {
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Logs.CountAsync();
        }
    }
}

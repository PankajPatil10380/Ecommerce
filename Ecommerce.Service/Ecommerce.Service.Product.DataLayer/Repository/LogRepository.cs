using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "InsertLog";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@LogLevel", log.LogLevel));
            command.Parameters.Add(new SqlParameter("@Action", log.Action));
            command.Parameters.Add(new SqlParameter("@Message", (object?)log.Message ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@TableName", (object?)log.TableName ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@RecordId", (object?)log.RecordId ?? DBNull.Value));
            command.Parameters.Add(new SqlParameter("@CreatedDate", log.CreatedDate != default ? log.CreatedDate : (object)DBNull.Value));

            await command.ExecuteNonQueryAsync();
            return true;
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.Logs.CountAsync();
        }

        public async Task<List<Log>> GetLogsAsync(int count = 50)
        {
            return await _context.Logs
                .OrderByDescending(l => l.CreatedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Common.Entities;
using Ecommerce.Common.Interfaces;

namespace Ecommerce.Common.Services
{
    public interface ILoggingService
    {
        Task LogAsync(string logLevel, string action, string message, string? tableName = null, int? recordId = null);
        Task<int> GetLogsCountAsync();
        Task<List<Log>> GetLogsAsync(int count = 50);
    }

    public class LoggingService : ILoggingService
    {
        private readonly ILogRepository _logRepository;

        public LoggingService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task LogAsync(string logLevel, string action, string message, string? tableName = null, int? recordId = null)
        {
            var log = new Log
            {
                LogLevel = logLevel,
                Action = action,
                Message = message,
                TableName = tableName,
                RecordId = recordId,
                CreatedDate = DateTime.Now
            };

            await _logRepository.InsertLogAsync(log);
        }

        public async Task<int> GetLogsCountAsync()
        {
            return await _logRepository.GetCountAsync();
        }

        public async Task<List<Log>> GetLogsAsync(int count = 50)
        {
            return await _logRepository.GetLogsAsync(count);
        }
    }
}

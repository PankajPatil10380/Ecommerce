using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Common.Entities;

namespace Ecommerce.Common.Interfaces
{
    public interface ILogRepository
    {
        Task<bool> InsertLogAsync(Log log);
        Task<int> GetCountAsync();
        Task<List<Log>> GetLogsAsync(int count = 50);
    }
}

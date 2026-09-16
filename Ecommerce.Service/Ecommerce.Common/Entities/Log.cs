using System;

namespace Ecommerce.Common.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string LogLevel { get; set; }
        public string Action { get; set; }
        public string Message { get; set; }
        public string TableName { get; set; }
        public int? RecordId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}

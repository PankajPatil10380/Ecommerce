using System.Collections.Generic;

namespace Ecommerce.Service.Product.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public int SubCategoryId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int GenderId { get; set; }
        public bool IsActive { get; set; }

        public virtual SubProductCategory SubCategory { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}

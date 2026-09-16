using System.Collections.Generic;

namespace Ecommerce.Service.Product.Domain.Entities
{
    public class SubProductCategory
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string SubCategory { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public virtual ProductCategory Category { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

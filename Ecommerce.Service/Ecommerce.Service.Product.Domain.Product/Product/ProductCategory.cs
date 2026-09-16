using System.Collections.Generic;

namespace Ecommerce.Service.Product.Domain.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<SubProductCategory> SubCategories { get; set; } = new List<SubProductCategory>();
    }
}

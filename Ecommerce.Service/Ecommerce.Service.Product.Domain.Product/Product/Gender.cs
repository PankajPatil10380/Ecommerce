using System.Collections.Generic;

namespace Ecommerce.Service.Product.Domain.Entities
{
    public class Gender
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}

using System;

namespace Ecommerce.Service.Product.BusinessLayer.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int GenderId { get; set; }
        public bool IsActive { get; set; }
    }
}

using System.Collections.Generic;

namespace Ecommerce.UI.ViewModel
{
    public class ProductViewModel
    {
        public ProductDto Product { get; set; } = new ProductDto();
        public List<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public List<SubProductCategory> SubCategories { get; set; } = new List<SubProductCategory>();
        public List<Gender> Genders { get; set; } = new List<Gender>();
        public int? SelectedCategoryId { get; set; }
        public int? SelectedSubCategoryId { get; set; }
        public int? SelectedGenderId { get; set; }
        public List<ProductDto> FilteredProducts { get; set; } = new List<ProductDto>();
    }
}

namespace Catalog.API.Models.DTO
{
    public class ProductDetailForUpdateDTO
    {
        public ProductDTO product { get; set; }
        public ProductCategoryDTO? device { get; set; }
        public ProductCategoryDTO? brand { get; set; }
        public List<ProductCategoryDTO>? color { get; set; }
    }
}

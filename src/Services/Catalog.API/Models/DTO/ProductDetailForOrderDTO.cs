namespace Catalog.API.Models.DTO
{
    public class ProductDetailForOrderDTO
    {
        public ProductDTO product { get; set; }
        public List<ProductCategoryDTO> color { get; set; } = default!;
    }
}

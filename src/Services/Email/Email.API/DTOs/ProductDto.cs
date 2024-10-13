namespace Email.API.DTOs
{
    public class ProductResponse
    {
        public ProductDto Product { get; set; }
    }
    public class ProductDto
    {
        public ProductDetailDto Product { get; set; }
        public object Device { get; set; }
        public object Brand { get; set; }
        public List<string> Color { get; set; } = new List<string>();
    }

    public class ProductDetailDto
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Status { get; set; }
        public List<string> ProductCategories { get; set; } = new List<string>();
    }
}

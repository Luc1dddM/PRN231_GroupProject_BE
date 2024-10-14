using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.DTO
{
    public class ProductDTO
    {
        public string ProductId { get; set; }
        public string Name { get; set; } = default!;
        public float Price { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ImageUrl { get; set; } = default!;
    }
}

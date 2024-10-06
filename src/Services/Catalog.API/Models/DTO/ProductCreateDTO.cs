using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.DTO
{
    public class ProductCreateDTO
    {
        public string Name { get; set; } = default!;
        public float Price { get; set; } = default!;
        public string Description { get; set; } = default!;
        public IFormFile Image { get; set; } = default!;
        public string Brand { get; set; } = default!;
        public string Device {  get; set; } = default!;
        public string Color {  get; set; } = default!;
        public int Quantity { get; set; } = default!;
        public bool Status { get; set; } = default!;
    }
}

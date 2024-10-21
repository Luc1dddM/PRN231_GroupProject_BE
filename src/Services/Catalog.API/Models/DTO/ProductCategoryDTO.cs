using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.DTO
{
    public class ProductCategoryDTO
    {
        public string ProductCategoryId { get; set; } = default!;
        public string CategoryId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string ColorName { get; set; } = default!;
        public int Quantity { get; set; }
        public bool Status { get; set; }
    }
}

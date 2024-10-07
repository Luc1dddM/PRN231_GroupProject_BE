using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.DTO
{
    public class ProductCategoryDTO
    {
        public string ProductCategoryId { get; set; } = default!;
        public string CategoryId { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public int Quantity { get; set; }
        public bool Status { get; set; }
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string Updatedby { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
    }
}

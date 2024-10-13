using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models
{
    public class ProductCategory
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductCategoryId { get; set; } = default!;
        [Required]
        public string CategoryId { get; set; } = default!;
        [ForeignKey("CategoryId")]
        [Required]
        public string ProductId { get; set; } = default!;
        [ForeignKey("ProductId")]
        [Required]
        public int Quantity { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string Updatedby { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
        public virtual Category Category { get; set; } = default!;
        public virtual Product Product { get; set; } = default!;
    }
}

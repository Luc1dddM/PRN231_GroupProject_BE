using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models
{
    public class Product
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductId { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public float Price { get; set; } = default!;
        [Required]
        public string Description { get; set; } = default!;
        [Required]
        public string ImageUrl { get; set; } = default!;
        public string CreateBy { get; set; } = default!;
        public DateTime CreateDate { get; set; } = default!;
        public string UpdateBy { get; set; } = default!;
        public DateTime UpdateDate { get; set; } = default!;
        [Required]
        public bool Status { get; set; } = default!;

        public virtual List<ProductCategory> ProductCategories { get; set; } = default!;
    }
}

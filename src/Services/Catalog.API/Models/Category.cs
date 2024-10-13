using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Catalog.API.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string CategoryId { get; set; }
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        public string Type { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = default!;
        public string UpdatedBy { get; set; } = default!;
        public DateTime UpdatedAt { get; set; } = default!;
        [Required]
        public bool Status { get; set; } = default!;
        public virtual List<ProductCategory> ProductCategories { get; set; } = default!;
    }
}

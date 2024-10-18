using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models.DTO
{
    public class CategoryDTO
    {
        public string CategoryId { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = default!;
        public string UpdatedBy { get; set; } = default!;
        public DateTime UpdatedAt { get; set; } = default!;
        public bool Status { get; set; } = default!;
    }
}

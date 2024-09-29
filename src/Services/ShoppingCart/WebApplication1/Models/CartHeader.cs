using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.API.Models
{
    public class CartHeader
    {
        [Key]
        public int Id { get; set; }

        public string CartHeaderId { get; set; } = null!;
        
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        
        public virtual List<CartDetail> CartDetails { get; set; } = new();
        
        public double TotalPrice => CartDetails.Sum(c => c.Price * c.Quantity);
    }
}

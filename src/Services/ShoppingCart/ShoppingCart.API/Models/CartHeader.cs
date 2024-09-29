namespace ShoppingCart.API.Models
{
    [Index(nameof(CartHeader.CartHeaderId), IsUnique = true)]
    public class CartHeader
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string CartHeaderId { get; set; } = null!;
        
        public DateTime CreatedDate { get; set; }

        public string CreatedBy { get; set; } = null!;

        public DateTime? UpdatedDate { get; set; }

        public string? UpdatedBy { get; set; }
        
        public List<CartDetail> CartDetails { get; set; } = new();
        
        public double TotalPrice => CartDetails.Sum(c => c.Price * c.Quantity);
    }
}

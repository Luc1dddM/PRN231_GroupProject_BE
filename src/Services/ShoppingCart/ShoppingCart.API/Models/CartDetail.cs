namespace ShoppingCart.API.Models
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }
        public string CartDetailId { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = null!;
        public double Price { get; set; } = default!;
        public string CartHeaderId { get; set; } = null!;
        public CartHeader CartHeader { get; set; } = null!;
        [NotMapped]
        public int NumberInStock { get; set; } = default!;
    }
}

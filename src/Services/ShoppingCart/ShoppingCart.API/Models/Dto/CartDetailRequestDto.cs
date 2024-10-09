namespace ShoppingCart.API.Models.Dto
{
    public class CartDetailRequestDto
    {
        public string ProductId { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public int Quantity { get; set; } = default!;
        public string Color { get; set; } = null!;
        public double Price { get; set; } = default!;
    }
}

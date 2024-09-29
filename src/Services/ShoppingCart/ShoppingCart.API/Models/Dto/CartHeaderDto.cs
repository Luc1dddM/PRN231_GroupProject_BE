namespace ShoppingCart.API.Models.Dto
{
    public class CartHeaderDto
    {
        public string CartHeaderId { get; set; } = default!;
        public double TotalPrice { get; set; }
    }
}

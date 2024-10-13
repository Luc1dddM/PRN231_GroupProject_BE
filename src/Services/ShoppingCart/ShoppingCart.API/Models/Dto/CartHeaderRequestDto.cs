namespace ShoppingCart.API.Models.Dto
{
    public class CartHeaderRequestDto
    {
        public string UserId { get; set; } = null!;

        public List<CartDetailRequestDto> CartDetails { get; set; } = new();

    }
}

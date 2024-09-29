namespace ShoppingCart.API.Models.Dto
{
    public class CartDto
    {
        public CartHeaderDto CartHeader { get; set; } = default!;
        public ICollection<CartDetailDto> CartDetails { get; set; } = [];
    }
}

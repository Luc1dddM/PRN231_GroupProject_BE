namespace ShoppingCart.API.ShoppingCart.GetCart
{

    public record GetCartResponse(CartHeaderDto CartHeader, ICollection<CartDetailDto> CartDetails);

    public class GetCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/cart/{userId}", async (string userId, ISender sender) =>
            {
                var result = await sender.Send(new GetCartQuery(userId));

                var response = new GetCartResponse(result.Cart.CartHeader, result.Cart.CartDetails);

                return response;
            })
            .WithName("GetCartByUserId")
            .Produces<GetCartResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Cart By User Id")
            .WithDescription("Get Cart By User Id");
        }
    }
}

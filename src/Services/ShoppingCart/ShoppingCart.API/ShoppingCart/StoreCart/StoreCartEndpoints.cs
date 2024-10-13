

namespace ShoppingCart.API.ShoppingCart.StoreCart
{
    public record StoreBasketRequest(CartHeader CartHeader);
    public record StoreBasketResponse(BaseResponse<CartDto> Response);

    public class StoreCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/cart", async (StoreBasketRequest request, ISender sender) =>
            {

                var command = request.Adapt<StoreCartCommand>();

                var result = await sender.Send(command);

                return Results.Ok(new StoreBasketResponse(result.Result));

            })
            .WithName("AddToCart")
            .Produces<StoreBasketResponse>(StatusCodes.Status200OK)
            .WithSummary("Add Product To Cart")
            .WithDescription("Add Product To Cart");
        }
    }
}

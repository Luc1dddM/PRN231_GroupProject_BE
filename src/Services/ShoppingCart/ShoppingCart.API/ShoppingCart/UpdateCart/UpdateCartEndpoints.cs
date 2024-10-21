
using ShoppingCart.API.ShoppingCart.StoreCart;

namespace ShoppingCart.API.ShoppingCart.UpdateCart
{
    public record UpdateBasketRequest(CartHeader CartHeader);
    public record UpdateBasketResponse(BaseResponse<CartDto> Response);

    public class UpdateCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/cart", async (UpdateBasketRequest request, ISender sender) =>
            {

                var command = request.Adapt<UpdateCartCommand>();

                var result = await sender.Send(command);

                return Results.Ok(new UpdateBasketResponse(result.Result));

            })
            .WithName("UpdateCart")
            .Produces<UpdateBasketResponse>(StatusCodes.Status200OK)
            .WithSummary("Update Cart")
            .WithDescription("Update Cart");
        }
    }
}

using ShoppingCart.API.ShoppingCart.StoreCart;

namespace ShoppingCart.API.ShoppingCart.DeleteCart
{

    public record DeleteCartResponse(BaseResponse<object> Response);

    public class DeleteCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/cart/{userId}", async (string userId, ISender sender) =>
            {

                var result = await sender.Send(new DeleteCartCommand(userId));


                return Results.Ok(new DeleteCartResponse(result.Result));

            })
            .WithName("DeleteCartDetails")
            .Produces<DeleteCartResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Cart Details")
            .WithDescription("Delete Cart Details");
        }
    }
}

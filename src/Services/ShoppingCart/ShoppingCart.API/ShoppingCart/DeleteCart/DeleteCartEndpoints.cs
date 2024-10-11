using ShoppingCart.API.ShoppingCart.StoreCart;

namespace ShoppingCart.API.ShoppingCart.DeleteCart
{

    public record DeleteCartResponse(BaseResponse<object> Response);

    public class DeleteCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/cart/{cartDetailId}", async (string cartDetailId, ISender sender) =>
            {
                try
                {
                    var result = await sender.Send(new DeleteCartCommand(cartDetailId));

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new DeleteCartResponse(result.Result));
                    }
                    return Results.BadRequest(new DeleteCartResponse(result.Result));

                }
                catch (Exception e)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<object>
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };

                    return Results.Json(new DeleteCartResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }

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

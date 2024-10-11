using ShoppingCart.API.ShoppingCart.StoreCart;

namespace ShoppingCart.API.ShoppingCart.GetCart
{

    public record GetCartResponse(BaseResponse<CartDto> Response);

    public class GetCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/cart/{userId}", async (string userId, ISender sender) =>
            {
                try
                {
                    var result = await sender.Send(new GetCartQuery(userId));

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new GetCartResponse(result.Result));
                    }

                    return Results.BadRequest(new GetCartResponse(result.Result));
                }
                catch (Exception ex)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };

                    return Results.Json(new GetCartResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }

            })
            .WithName("GetCartByUserId")
            .Produces<GetCartResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Cart By User Id")
            .WithDescription("Get Cart By User Id");
        }
    }
}

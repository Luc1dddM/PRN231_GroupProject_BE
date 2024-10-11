

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
                try
                {
                    var command = request.Adapt<StoreCartCommand>();

                    var result = await sender.Send(command);

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new StoreBasketResponse(result.Result));
                    }
                    return Results.BadRequest(new StoreBasketResponse(result.Result));
                }
                catch (Exception ex)
                {

                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<CartDto>
                    {
                        IsSuccess = false,
                        Message = ex.Message
                    };

                    return Results.Json(new StoreBasketResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }

            })
            .WithName("AddToCart")
            .Produces<StoreBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Product To Cart")
            .WithDescription("Add Product To Cart");
        }
    }
}

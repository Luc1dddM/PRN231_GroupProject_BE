using ShoppingCart.API.ShoppingCart.DeleteCart;

namespace ShoppingCart.API.ShoppingCart.CheckoutCart
{
    public record CheckoutCartRequest(CartCheckoutDto CartCheckoutDto);
    public record CheckoutCartResponse(BaseResponse<object> Response);


    public class CheckoutCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/cart/checkout", async (CheckoutCartRequest request, ISender sender) =>
            {
                try
                {
                    var command = request.Adapt<CheckoutCartCommand>();

                    var result = await sender.Send(command);

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new CheckoutCartResponse(result.Result));
                    }
                    return Results.BadRequest(new CheckoutCartResponse(result.Result));
                }
                catch (Exception e)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<object>
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };

                    return Results.Json(new CheckoutCartResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }

            })
            .WithName("CheckoutBasket")
            .Produces<CheckoutCartResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket");
        }
    }
}

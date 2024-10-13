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

                var command = request.Adapt<CheckoutCartCommand>();

                var result = await sender.Send(command);

                return Results.Ok(new CheckoutCartResponse(result.Result));

            })
            .WithName("CheckoutBasket")
            .Produces<CheckoutCartResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket");
        }
    }
}

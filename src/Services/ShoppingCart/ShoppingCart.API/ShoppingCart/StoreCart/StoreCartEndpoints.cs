namespace ShoppingCart.API.ShoppingCart.StoreCart
{
    public record StoreBasketRequest(CartHeader CartHeader);
    public record StoreBasketResponse(bool IsSuccess, string Message);

    public class StoreCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/cart", async (StoreBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<StoreCartCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<StoreBasketResponse>();

                if (result.IsSuccess)
                {
                    return Results.Ok(new StoreBasketResponse(true, response.Message));
                }
                return Results.BadRequest(new StoreBasketResponse(false, response.Message));
            })
            .WithName("AddToCart")
            .Produces<StoreBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Product To Cart")
            .WithDescription("Add Product To Cart");
        }
    }
}

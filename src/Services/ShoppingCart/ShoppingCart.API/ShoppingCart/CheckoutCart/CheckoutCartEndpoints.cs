﻿namespace ShoppingCart.API.ShoppingCart.CheckoutCart
{
    public record CheckoutCartRequest(CartCheckoutDto CartCheckoutDto);
    public record CheckoutCartResponse(bool IsSuccess);


    public class CheckoutCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/cart/checkout", async (CheckoutCartRequest request, ISender sender) =>
            {
                var command = request.Adapt<CheckoutcartCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CheckoutCartResponse>();

                return Results.Ok(response);
            })
            .WithName("CheckoutBasket")
            .Produces<CheckoutCartResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket");
        }
    }
}

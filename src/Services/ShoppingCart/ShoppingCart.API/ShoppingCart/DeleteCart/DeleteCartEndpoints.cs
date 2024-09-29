namespace ShoppingCart.API.ShoppingCart.DeleteCart
{

    public record DeleteCartResponse(bool IsSuccess);

    public class DeleteCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/cart/{cartDetailId}", async (string cartDetailId, ISender sender) =>
            {
                var result = await sender.Send(new DeleteCartCommand(cartDetailId));

                var response = result.Adapt<DeleteCartResponse>();

                return Results.Ok(response);
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

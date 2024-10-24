namespace ShoppingCart.API.ShoppingCart.GetCart
{

    public record GetCartResponse(BaseResponse<CartDto> Response);

    public class GetCartEndpoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/cart", async (ISender sender) =>
            {

                var result = await sender.Send(new GetCartQuery());


                return Results.Ok(new GetCartResponse(result.Result));

            })
            .WithName("GetCartByUserId")
            .Produces<GetCartResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Cart By User Id")
            .WithDescription("Get Cart By User Id");
        }
    }
}

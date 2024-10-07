
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.Endpoints
{

    //public record GetOrdersRequest();
    public record GetOrdersResponse(IEnumerable<OrderDto> Orders);

    public class GetOrders : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async (ISender sender) =>
            {
                var result = await sender.Send(new GetOrdersQuery());

                var response = result.Adapt<GetOrdersResponse>();

                return Results.Ok(response);
            })
            .WithName("GetOrders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Orders")
            .WithDescription("Get Orders");
        }
    }
}

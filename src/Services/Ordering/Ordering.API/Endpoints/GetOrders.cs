
using BuildingBlocks.Models;
using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.API.Endpoints
{

    //public record GetOrdersRequest();
    public record GetOrdersResponse(BaseResponse<IEnumerable<OrderDto>> Response);

    public class GetOrders : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders", async (ISender sender) =>
            {

                var result = await sender.Send(new GetOrdersQuery());

                return Results.Ok(new GetOrdersResponse(result.Result));
            
            })
            .WithName("GetOrders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .WithSummary("Get Orders")
            .WithDescription("Get Orders");
        }
    }
}

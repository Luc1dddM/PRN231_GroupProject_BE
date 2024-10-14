using BuildingBlocks.Models;
using Ordering.Application.Orders.Queries.GetOrders;
using Ordering.Application.Orders.Queries.Statistic;

namespace Ordering.API.Endpoints
{

    //public record GetOrdersRequest();
    public record StatisticResponse(BaseResponse<StatisticDTO> Result);

    public class StatisticEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/statistic", async (ISender sender) =>
            {
                var result = await sender.Send(new StatisticQuery());

                    return Results.Ok(new StatisticResponse(result.Result));

            })
            .WithName("Statistic")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Statistic")
            .WithDescription("Statistic");
        }
    }
}

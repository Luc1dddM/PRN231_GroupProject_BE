
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
                try
                {
                    var result = await sender.Send(new GetOrdersQuery());

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new GetOrdersResponse(result.Result));
                    }

                    if (result.Result.Message.Contains("No Order Data."))
                    {
                        return Results.NotFound(new GetOrdersResponse(result.Result));
                    }
                    return Results.BadRequest(new GetOrdersResponse(result.Result));
                }
                catch (Exception e)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<IEnumerable<OrderDto>>
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };

                    return Results.Json(new GetOrdersResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }
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

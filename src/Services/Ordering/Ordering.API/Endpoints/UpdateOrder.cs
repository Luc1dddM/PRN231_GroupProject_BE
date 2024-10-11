using BuildingBlocks.Models;
using Ordering.Application.Orders.Commands.UpdateOrder;

namespace Ordering.API.Endpoints
{
    //- Accepts a UpdateOrderRequest.
    //- Maps the request to an UpdateOrderCommand.
    //- Sends the command for processing.
    //- Returns a success or error response based on the outcome.

    public record UpdateOrderRequest(OrderDtoUpdateRequest Order);
    public record UpdateOrderResponse(BaseResponse<OrderDto> Response);

    public class UpdateOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/orders", async (UpdateOrderRequest request, ISender sender) =>
            {
                try
                {
                    var command = request.Adapt<UpdateOrderCommand>();

                    var result = await sender.Send(command);

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new UpdateOrderResponse(result.Result));
                    }
                    return Results.BadRequest(new UpdateOrderResponse(result.Result));
                    
                }
                catch (Exception e)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<OrderDto>
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };

                    return Results.Json(new UpdateOrderResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("UpdateOrder")
            .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Order")
            .WithDescription("Update Order");
        }
    }
}

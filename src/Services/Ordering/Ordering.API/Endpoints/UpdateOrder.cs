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

                var command = request.Adapt<UpdateOrderCommand>();

                var result = await sender.Send(command);

                return Results.Ok(new UpdateOrderResponse(result.Result));

            })
            .WithName("UpdateOrder")
            .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
            .WithSummary("Update Order")
            .WithDescription("Update Order");
        }
    }
}

using BuildingBlocks.Models;
using Ordering.Application.Orders.Commands.DeleteOrder;

namespace Ordering.API.Endpoints
{
    //- Accepts the order ID as a parameter.
    //- Constructs a DeleteOrderCommand.
    //- Sends the command using MediatR.
    //- Returns a success or not found response.

    //public record DeleteOrderRequest(Guid Id);
    public record DeleteOrderResponse(BaseResponse<object> Response);


    public class DeleteOrder : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {   //the "orderId" in the route must be identical as the one need to be send 
            app.MapDelete("/orders/{orderId}", async (Guid orderId, ISender sender) =>
            {
                try
                {
                    var result = await sender.Send(new DeleteOrderCommand(orderId));

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new DeleteOrderResponse(result.Result));
                    }
                    return Results.BadRequest(new DeleteOrderResponse(result.Result));
                }
                catch (Exception e)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<object>
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };

                    return Results.Json(new DeleteOrderResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("DeleteOrder")
            .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Order")
            .WithDescription("Delete Order");
        }
    }
}

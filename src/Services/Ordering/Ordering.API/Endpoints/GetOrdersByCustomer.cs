
using BuildingBlocks.Models;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ordering.API.Endpoints
{
    //- Accepts a customer ID.
    //- Uses a GetOrdersByCustomerQuery to fetch orders.
    //- Returns the list of orders for that customer.

    //public record GetOrdersByCustomerRequest(Guid CustomerId);
    public record GetOrdersByCustomerResponse(BaseResponse<IEnumerable<OrderDto>> Response);

    public class GetOrdersByCustomer : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/customer/{customerId}", async (Guid customerId, ISender sender) =>
            {
                try
                {
                    var result = await sender.Send(new GetOrdersByCustomerQuery(customerId));

                    if (result.Result.IsSuccess)
                    {
                        return Results.Ok(new GetOrdersByCustomerResponse(result.Result));
                    }
                    if (result.Result.Message.Contains($"User with Id {customerId} does not have any order yet."))
                    {
                        return Results.NotFound(new GetOrdersByCustomerResponse(result.Result));
                    }
                    return Results.BadRequest(new GetOrdersByCustomerResponse(result.Result));
                }
                catch (Exception e)
                {
                    // Return 500 with the custom BaseResponse format
                    var errorResponse = new BaseResponse<IEnumerable<OrderDto>>
                    {
                        IsSuccess = false,
                        Message = e.Message
                    };

                    return Results.Json(new GetOrdersByCustomerResponse(errorResponse), statusCode: StatusCodes.Status500InternalServerError);
                }

            })
            .WithName("GetOrdersByCustomer")
            .Produces<GetOrdersByCustomerResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Orders By Customer")
            .WithDescription("Get Orders By Customer");
        }
    }
}

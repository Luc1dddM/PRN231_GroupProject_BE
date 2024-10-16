
using BuildingBlocks.Models;
using Ordering.Application.Orders.Queries.GetOrdersByCustomer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ordering.API.Endpoints
{
    //- Accepts a customer ID.
    //- Uses a GetOrdersByCustomerQuery to fetch orders.
    //- Returns the list of orders for that customer.

    //public record GetOrdersByCustomerRequest(Guid CustomerId);
    public record GetOrdersByCustomerResponse(BaseResponse<PaginatedList<OrderDto>> Response);

    public class GetOrdersByCustomer : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/orders/customer/{customerId}", async (Guid customerId, [AsParameters] GetListOrderParamsDto request, ISender sender) =>
            {

                var result = await sender.Send(new GetOrdersByCustomerQuery(customerId, request));

                return Results.Ok(new GetOrdersByCustomerResponse(result.Result));

            })
            .WithName("GetOrdersByCustomer")
            .Produces<GetOrdersByCustomerResponse>(StatusCodes.Status200OK)
            .WithSummary("Get Orders By Customer")
            .WithDescription("Get Orders By Customer");
        }
    }
}

using BuildingBlocks.Models;

namespace Ordering.Application.Orders.Queries.GetOrders
{
    public record GetOrdersQuery() : IQuery<GetOrdersResult>;

    public record GetOrdersResult(BaseResponse<IEnumerable<OrderDto>> Result);
}

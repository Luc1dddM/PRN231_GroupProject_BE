namespace Ordering.Application.Orders.Queries.GetOrders
{
    public record GetOrdersQuery(GetListOrderParamsDto Param) : IQuery<GetOrdersResult>;

    public record GetOrdersResult(BaseResponse<PaginatedList<OrderDto>> Result);
}

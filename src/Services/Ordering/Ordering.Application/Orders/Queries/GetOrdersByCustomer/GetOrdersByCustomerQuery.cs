namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer
{
    public record GetOrdersByCustomerQuery(Guid CustomerId, GetListOrderParamsDto Param) : IQuery<GetOrdersByCustomerResult>;

    public record GetOrdersByCustomerResult(BaseResponse<PaginatedList<OrderDto>> Result);
}

using Ordering.Application.Orders.Queries.GetOrders;

namespace Ordering.Application.Orders.Queries.GetOrdersByCustomer
{
    public class GetOrdersByCustomerHandler : IQueryHandler<GetOrdersByCustomerQuery, GetOrdersByCustomerResult>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdersByCustomerHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrdersByCustomerResult> Handle(GetOrdersByCustomerQuery query, CancellationToken cancellationToken)
        {
            // get orders by customer using dbContext
            // return result
            try
            {
                var orders = await _context.Orders
                            .Include(o => o.OrderItems)
                                        .AsNoTracking()
                                        .Where(o => o.CustomerId == CustomerId.Of(query.CustomerId))
                                        .ToListAsync(cancellationToken);
                if (orders.Count == 0)
                {
                    return new GetOrdersByCustomerResult(new BaseResponse<IEnumerable<OrderDto>>
                    {
                        IsSuccess = false, //list can have no item so this could be "true"
                        Message = $"User with Id {query.CustomerId} does not have any order yet."
                    });
                }
                return new GetOrdersByCustomerResult(new BaseResponse<IEnumerable<OrderDto>>
                {
                    IsSuccess = true,
                    Result = orders.ToOrderDtoList(),
                    Message = $"All Order Of User {query.CustomerId} Retrieve Successful."
                });
            }
            catch (Exception e)
            {
                return new GetOrdersByCustomerResult(new BaseResponse<IEnumerable<OrderDto>>
                {
                    IsSuccess = false,
                    Message = e.Message
                });
            }


        }
    }
}

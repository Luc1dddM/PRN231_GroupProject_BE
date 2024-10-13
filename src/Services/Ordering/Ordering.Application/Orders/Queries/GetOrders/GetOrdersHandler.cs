
using BuildingBlocks.Models;

namespace Ordering.Application.Orders.Queries.GetOrders
{
    public class GetOrdersHandler : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {
        private readonly IApplicationDbContext _context;

        public GetOrdersHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            // get all orders using dbContext
            // return result
            try
            {
                var orders = await _context.Orders
                            .Include(o => o.OrderItems)
                                        .AsNoTracking()
                                        .ToListAsync(cancellationToken);
                if (orders.Count == 0)
                {
                    //return new GetOrdersResult(new BaseResponse<IEnumerable<OrderDto>>
                    //{
                    //    IsSuccess = false, //list can have no item so this could be "true"
                    //    Message = "No Order Data."
                    //});
                    throw new NotFoundException("Order list has no data");
                }

                return new GetOrdersResult(new BaseResponse<IEnumerable<OrderDto>>
                {
                    IsSuccess = true,
                    Result = orders.ToOrderDtoList(),
                    Message = "All Order Retrieve Successful."
                });
            }
            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }
    }
}

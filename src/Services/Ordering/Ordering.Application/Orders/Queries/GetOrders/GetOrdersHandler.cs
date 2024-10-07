
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

            var orders = await _context.Orders
            .Include(o => o.OrderItems)
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

            return new GetOrdersResult(orders.ToOrderDtoList());
        }
    }
}

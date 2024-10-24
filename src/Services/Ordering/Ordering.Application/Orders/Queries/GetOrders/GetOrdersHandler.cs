
using BuildingBlocks.Models;
using Ordering.Domain.Enums;

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

            IQueryable<Order> orderQuery = _context.Orders.Include(o => o.OrderItems).AsNoTracking();

            // Check if there are any orders
            if (!await orderQuery.AnyAsync(cancellationToken))
            {
                throw new NotFoundException("No orders found in the database.");
            }

            // Step 1: Apply filters
            orderQuery = Filter(orderQuery, query.Param.Statuses, query.Param.PaymentMethods);

            // Step 2: Apply keyword search
            orderQuery = Search(orderQuery, query.Param.Keyword);

            // Step 3: Apply sorting
            orderQuery = SortOrder(query.Param.SortBy, query.Param.SortOrder, orderQuery);

            // Step 4: Get total count for pagination
            //var totalCount = await orderQuery.CountAsync(cancellationToken);

            // Step 5: Apply pagination
            var paginatedOrders = await PaginatedList<OrderDto>.CreateAsync(
                                                                orderQuery.Select(o=>o.ToOrderDto()), 
                                                                query.Param.PageNumber, 
                                                                query.Param.PageSize);

            if (!paginatedOrders.Items.Any())
            {
                throw new NotFoundException("No orders found matching the criteria.");
            }


            return new GetOrdersResult(new BaseResponse<PaginatedList<OrderDto>>
            {
                IsSuccess = true,
                Result = paginatedOrders,
                Message = "Orders retrieved successfully."
            });
        }



        //filter for order status and payment method
        private IQueryable<Order> Filter(IQueryable<Order> queryList, string[] statuses, string[] paymentMethods)
        {
            if (statuses != null && statuses.Length > 0 && statuses.Any(s => !string.IsNullOrWhiteSpace(s)))
            {
                var statusEnum = statuses.Select(s => Enum.Parse<OrderStatus>(s)).ToArray();

                queryList = queryList.Where(o => statusEnum.Contains(o.Status));
            }

            if (paymentMethods != null && paymentMethods.Length > 0 && paymentMethods.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                queryList = queryList.Where(o => paymentMethods.Contains(o.Payment));
            }

            return queryList;
        }


        //serach by address properties: LastName, Phone, EmailAddress
        private IQueryable<Order> Search(IQueryable<Order> queryList, string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                queryList = queryList.Where(o =>
                    o.ShippingAddress.LastName.ToLower().Contains(keyword) ||
                    o.ShippingAddress.Phone.Contains(keyword) ||
                    (o.ShippingAddress.EmailAddress != null && o.ShippingAddress.EmailAddress.ToLower().Contains(keyword)));
            }
            return queryList;
        }


        //sort by lastname, phone, and email
        private IQueryable<Order> SortOrder(string sortBy, string sortOrder, IQueryable<Order> list)
        {
            switch (sortBy.ToLower())
            {
                case "lastname":
                    list = sortOrder == "asc" ? list.OrderBy(o => o.ShippingAddress.LastName) : list.OrderByDescending(o => o.ShippingAddress.LastName);
                    break;
                case "phone":
                    list = sortOrder == "asc" ? list.OrderBy(o => o.ShippingAddress.Phone) : list.OrderByDescending(o => o.ShippingAddress.Phone);
                    break;
                case "email":
                    list = sortOrder == "asc" ? list.OrderBy(o => o.ShippingAddress.EmailAddress) : list.OrderByDescending(o => o.ShippingAddress.EmailAddress);
                    break;
                default:
                    list = list.OrderByDescending(o => o.CreatedAt);
                    break;
            }
            return list;
        }
    }
}

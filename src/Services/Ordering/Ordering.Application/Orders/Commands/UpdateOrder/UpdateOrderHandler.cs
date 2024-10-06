namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateOrderHandler> _logger;
        private const int MaxRetries = 3;

        public UpdateOrderHandler(IApplicationDbContext context, ILogger<UpdateOrderHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                //getting the Order to update
                var orderId = OrderId.Of(command.Order.Id);

                var order = await _context.Orders.Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken: cancellationToken);

                if (order is null)
                {
                    _logger.LogWarning($"Order not found for ID: {orderId.Value}");
                    throw new OrderNotFoundException(command.Order.Id);
                }



                //update the order with the new data from the Dto object: command.Order
                UpdateOrderWithNewValues(order, command.Order);

                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);

                return new UpdateOrderResult(true);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private Order UpdateOrderWithNewValues(Order order, OrderDto orderDto)
        {
            var updatedShippingAddress = Address.Of(orderDto.ShippingAddress.FirstName,
                                                    orderDto.ShippingAddress.LastName,
                                                    orderDto.ShippingAddress.Phone,
                                                    orderDto.ShippingAddress.EmailAddress,
                                                    orderDto.ShippingAddress.AddressLine,
                                                    orderDto.ShippingAddress.City,
                                                    orderDto.ShippingAddress.District,
                                                    orderDto.ShippingAddress.Ward);

            order.Update(
            shippingAddress: updatedShippingAddress,
            status: orderDto.Status);

            return order;
        }

    }
}


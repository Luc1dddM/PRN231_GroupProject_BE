using BuildingBlocks.Messaging.Events;
using MassTransit;
using Ordering.Application.Orders.Commands.CreateOrder;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.EventHandlers.Integration
{
    public class CartCheckoutEventHandler : IConsumer<CartCheckoutEvent>
    {
        private readonly ISender _sender;
        private readonly ILogger<CartCheckoutEventHandler> _logger;
        public CartCheckoutEventHandler(ISender sender, ILogger<CartCheckoutEventHandler> logger)
        {
            _logger = logger;
            _sender = sender;
        }

        public async Task Consume(ConsumeContext<CartCheckoutEvent> context)
        {
            //create new ordewr
            _logger.LogInformation("Integration Event Handled: {IntegrationEvent}", context.Message.GetType().Name);

            var command = MapToCreateOrderCommand(context.Message);
            await _sender.Send(command);
            throw new NotImplementedException();
        }


        private CreateOrderCommand MapToCreateOrderCommand(CartCheckoutEvent message)
        {
            // Create full order with incoming event data
            var addressDto = new AddressDto // all of the incomming data store in record for each part
                (
                    message.FirstName,
                    message.LastName,
                    message.Phone,
                    message.EmailAddress,
                    message.AddressLine,
                    message.City,
                    message.District,
                    message.Ward
                );

            var paymentDto = new PaymentDto
                (
                    message.CardName,
                    message.CardNumber,
                    message.Expiration,
                    message.CVV,
                    message.PaymentMethod
                );

            var orderId = Guid.NewGuid();

            var orderItems = message.CartItems.Select(item => new OrderItemDto(
                    OrderId: orderId,
                    ProductId: Guid.Parse(item.ProductId),
                    Quantity: item.Quantity,
                    Price: item.Price,
                    Color: item.Color
                )).ToList();


            var orderDto = new OrderDtoRequest
                (
                    CustomerId: message.CustomerId,
                    ShippingAddress: addressDto,
                    Payment: paymentDto,
                    Status: OrderStatus.Pending,
                    OrderItems: orderItems,
                    CouponCode: message.CouponCode
                );

            return new CreateOrderCommand(orderDto);
        }
    }
}

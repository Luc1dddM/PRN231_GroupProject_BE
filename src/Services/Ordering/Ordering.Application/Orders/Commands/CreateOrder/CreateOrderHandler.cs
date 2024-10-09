using BuildingBlocks.Messaging.Events;
using Coupon.Grpc;
using Mapster;
using MassTransit;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IApplicationDbContext _context;

        private readonly CouponProtoService.CouponProtoServiceClient _couponProto;
        private readonly IPublishEndpoint _publishEndpoint;
        
        public CreateOrderHandler(
            IApplicationDbContext context, 
            CouponProtoService.CouponProtoServiceClient couponProto,
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _couponProto = couponProto;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            // Add debug logging
            Console.WriteLine($"CouponCode in command: {command.Order.CouponCode}");
            //create Order entity from command object
            var order = await CreateNewOrder(command.Order);

            //save to database
            _context.Orders.Add(order);

            await _context.SaveChangesAsync(cancellationToken);

            //return result 
            return new CreateOrderResult(order.Id.Value);
        }



        private async Task<Order> CreateNewOrder(OrderDtoRequest orderDto)
        {
            var shippingAddress = Address.Of(orderDto.ShippingAddress.FirstName,
                                             orderDto.ShippingAddress.LastName,
                                             orderDto.ShippingAddress.Phone,
                                             orderDto.ShippingAddress.EmailAddress,
                                             orderDto.ShippingAddress.AddressLine,
                                             orderDto.ShippingAddress.City,
                                             orderDto.ShippingAddress.District,
                                             orderDto.ShippingAddress.Ward);

            var newOrder = Order.Create(
                id: OrderId.Of(Guid.NewGuid()),
                customerId: CustomerId.Of(orderDto.CustomerId),
                shippingAddress: shippingAddress,
                payment: Payment.Of(orderDto.Payment.CardName,
                                    orderDto.Payment.CardNumber,
                                    orderDto.Payment.Expiration,
                                    orderDto.Payment.Cvv,
                                    orderDto.Payment.PaymentMethod),
                couponId: null
                );

            foreach (var orderItemDto in orderDto.OrderItems)
            {
                newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.Quantity, orderItemDto.Price, orderItemDto.Color);
            }

            var totalPrice = newOrder.TotalPrice;

            if (!string.IsNullOrEmpty(orderDto.CouponCode))
            {
                var coupon = await _couponProto.GetCouponAsync(new GetCouponRequest { CouponCode = orderDto.CouponCode });

                if (coupon != null && coupon.Status)
                {
                    newOrder.CouponId = coupon.CouponId;
                    // Verify if the order meets coupon conditions
                    if (totalPrice >= (decimal)coupon.MinAmount && totalPrice <= (decimal)coupon.MaxAmount)
                    {
                        //ApplyCoupon() method is contain inside the Order domain
                        newOrder.ApplyCoupon((decimal)coupon.DiscountAmount); //apply the discount to the order
                    }
                }
            }

            return newOrder;
        }
    }
}

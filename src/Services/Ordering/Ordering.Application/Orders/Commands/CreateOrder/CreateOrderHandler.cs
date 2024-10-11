using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Models;
using Coupon.Grpc;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Http;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ordering.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly CouponProtoService.CouponProtoServiceClient _couponProto;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateOrderHandler(
            IApplicationDbContext context,
            CouponProtoService.CouponProtoServiceClient couponProto,
            IPublishEndpoint publishEndpoint,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _couponProto = couponProto;
            _contextAccessor = contextAccessor;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Try to get userId from HTTP context first
                var userId = _contextAccessor.HttpContext?.Request.Headers["UserId"].ToString();

                // If userId is null (RabbitMQ scenario), use CustomerId from the command
                if (string.IsNullOrEmpty(userId))
                {
                    userId = command.Order.CustomerId.ToString();
                }

                // Add debug logging
                Console.WriteLine($"CouponCode in command: {command.Order.CouponCode}");
                //create Order entity from command object
                var order = await CreateNewOrder(command.Order, userId);

                //save to database
                _context.Orders.Add(order);

                await _context.SaveChangesAsync(cancellationToken);

                
                //return result 
                return new CreateOrderResult(new BaseResponse<OrderDto>
                {
                    IsSuccess = true,
                    Result = order.ToOrderDto(),
                    Message = "Order Created Successful."
                });
            }
            catch (Exception e)
            {

                return new CreateOrderResult(new BaseResponse<OrderDto>
                {
                    IsSuccess = false,
                    Message = e.Message
                });
            }

        }



        private async Task<Order> CreateNewOrder(OrderDtoRequest orderDto, string userId)
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
                orderId: OrderId.Of(Guid.NewGuid()),
                customerId: CustomerId.Of(Guid.Parse(userId)),
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
                newOrder.Add(ProductId.Of(orderItemDto.ProductId), orderItemDto.ProductCategoryId, orderItemDto.Quantity, orderItemDto.Price, orderItemDto.Color);
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

using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Events.DTO;
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
                var orderItem = MapOrderItemToReduceQuantity(order.OrderItems, userId);
                _publishEndpoint.Publish(orderItem);

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
            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            catch(BadRequestException e)
            {
                throw new BadRequestException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }

        }


        private ReduceQuantityEvent MapOrderItemToReduceQuantity(IReadOnlyList<OrderItem> orderItems, string userId)
        {

            var tmp = new List<ReduceQuantityDTO>();
            foreach (var item in orderItems)
            {
                var reduceQuantityDTO = new ReduceQuantityDTO
                {
                    productCategoryId = item.ProductCategoryId,
                    quantity = item.Quantity,
                    user = userId,
                    IsCancel = false
                };
                tmp.Add(reduceQuantityDTO);
            }
            var nEvent = new ReduceQuantityEvent()
            {
                listProductCategory = tmp
            };

            return nEvent;
        }



        private async Task<Order> CreateNewOrder(OrderDtoRequest orderDto, string userId)
        {
            ValidateOrderRequest(orderDto);

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
                payment: orderDto.Payment,
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

                        // Publish CouponQuantityUsedEvent after applying the coupon
                        var couponUsedEvent = new CouponQuantityUsedEvent
                        {
                            CouponCode = coupon.CouponCode,
                            QuantityUsed = 1 // Assuming 1 coupon used per order
                        };

                        // Publish the event
                        await _publishEndpoint.Publish(couponUsedEvent);
                    }
                }
                else
                {
                    throw new NotFoundException($"Coupon {orderDto.CouponCode} can not be found.");
                }
            }

            return newOrder;
        }



        private void ValidateOrderRequest(OrderDtoRequest orderDto)
        {
            if (orderDto == null)
                throw new BadRequestException("Order request cannot be null.");

            if (orderDto.ShippingAddress == null ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.FirstName) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.LastName) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.Phone) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.AddressLine) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.City) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.District) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.Ward))
            {
                throw new BadRequestException("All shipping address fields are required.");
            }

            if (orderDto.Payment == null)
            {
                throw new BadRequestException("Payment fields are required.");
            }

            if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            {
                throw new BadRequestException("Order must contain at least one item.");
            }

            if (orderDto.OrderItems.Any(item =>
                item.ProductId == Guid.Empty ||
                string.IsNullOrWhiteSpace(item.ProductCategoryId) ||
                item.Quantity <= 0 ||
                item.Price <= 0 ||
                string.IsNullOrWhiteSpace(item.Color)))
            {
                throw new BadRequestException("All order item fields are required and must be valid.");
            }
        }
    }
}

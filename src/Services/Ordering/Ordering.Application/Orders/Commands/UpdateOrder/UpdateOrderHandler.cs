using BuildingBlocks.Messaging.Events.DTO;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Models;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateOrderHandler> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPublishEndpoint _publishEndpoint;

        public UpdateOrderHandler(IApplicationDbContext context, 
            ILogger<UpdateOrderHandler> logger, 
            IHttpContextAccessor contextAccessor,
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _contextAccessor = contextAccessor;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext.Request.Headers["UserId"].ToString();

                //getting the Order to update
                var orderId = OrderId.Of(command.Order.EntityId);

                var order = await _context.Orders.Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.EntityId == orderId, cancellationToken: cancellationToken);

                if (order is null)
                {
                    _logger.LogWarning($"Order not found for ID: {orderId.Value}");
                    throw new OrderNotFoundException(command.Order.EntityId);
                }

                var oldStatus = order.Status;
                //update the order with the new data from the Dto object: command.Order
                UpdateOrderWithNewValues(order, command.Order);

                if (oldStatus != OrderStatus.Cancelled && order.Status == OrderStatus.Cancelled)
                {
                    _logger.LogInformation("Publish event to re add the quantity of product (foreach here or return a list of OrderItem)");
                    var list = MapOrderItemToReduceQuantity(order.OrderItems, userId);
                    _publishEndpoint.Publish(list);
                }

                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);

                return new UpdateOrderResult(new BaseResponse<OrderDto>
                {
                    IsSuccess = true,
                    Result = order.ToOrderDto(),
                    Message = "Order Updated Successful"
                });
            }
            catch (Exception e)
            {
                return new UpdateOrderResult(new BaseResponse<OrderDto>
                {
                    IsSuccess = false,
                    Message = e.Message
                });
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
                    IsCancel = true
                };
                tmp.Add(reduceQuantityDTO);
            }
            var nEvent = new ReduceQuantityEvent()
            {
                listProductCategory = tmp
            };

            return nEvent;
        }


        private Order UpdateOrderWithNewValues(Order order, OrderDtoUpdateRequest orderDto)
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


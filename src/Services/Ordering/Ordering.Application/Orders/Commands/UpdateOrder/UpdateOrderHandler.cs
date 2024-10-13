using BuildingBlocks.Models;
using Microsoft.AspNetCore.Http;
using Ordering.Domain.Enums;

namespace Ordering.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderHandler : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<UpdateOrderHandler> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public UpdateOrderHandler(IApplicationDbContext context, ILogger<UpdateOrderHandler> logger, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var userId = _contextAccessor.HttpContext.Request.Headers["UserId"].ToString();

                if (string.IsNullOrEmpty(userId))
                {
                    throw new NotFoundException("UserId did not have any value in the incoming request.");
                }

                //getting the Order to update
                var orderId = OrderId.Of(command.Order.EntityId);

                var order = await _context.Orders.Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.EntityId == orderId, cancellationToken: cancellationToken);

                if (order is null)
                {
                    _logger.LogWarning($"Order not found for ID: {orderId.Value}");
                    throw new NotFoundException($"Order with Id {orderId.Value} does not exist.");
                }

                var oldStatus = order.Status;
                //update the order with the new data from the Dto object: command.Order
                UpdateOrderWithNewValues(order, command.Order);

                if (oldStatus != OrderStatus.Cancelled && order.Status == OrderStatus.Cancelled)
                {
                    _logger.LogInformation("Publish event to re add the quantity of product (foreach here or return a list of OrderItem)");
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
            catch (NotFoundException e)
            {
                throw new NotFoundException(e.Message, e);
            }
            catch (BadRequestException e)
            {
                throw new BadRequestException(e.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }

        private Order UpdateOrderWithNewValues(Order order, OrderDtoUpdateRequest orderDto)
        {
            ValidateOrderRequest(orderDto, order.Status);
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


        private void ValidateOrderRequest(OrderDtoUpdateRequest orderDto, OrderStatus currentStatus)
        {
            if (orderDto == null)
                throw new BadRequestException("Order update request cannot be null.");

            if (orderDto.ShippingAddress == null ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.FirstName) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.LastName) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.Phone) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.EmailAddress) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.AddressLine) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.City) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.District) ||
                string.IsNullOrWhiteSpace(orderDto.ShippingAddress.Ward))
            {
                throw new BadRequestException("All shipping address fields are required.");
            }

            if (!Enum.IsDefined(typeof(OrderStatus), orderDto.Status))
            {
                throw new BadRequestException("Invalid order status.");
            }

            // Validate status transitions
            if (!IsValidStatusTransition(currentStatus, orderDto.Status))
            {
                throw new BadRequestException($"Invalid status transition from {currentStatus} to {orderDto.Status}.");
            }
        }


        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // Define valid status transitions
            switch (currentStatus)
            {
                case OrderStatus.Pending:
                    return newStatus == OrderStatus.Approved || newStatus == OrderStatus.Cancelled;
                case OrderStatus.Approved:
                    return newStatus == OrderStatus.Shipping || newStatus == OrderStatus.Cancelled;
                case OrderStatus.Shipping:
                    return newStatus == OrderStatus.Completed;
                case OrderStatus.Completed:
                case OrderStatus.Cancelled:
                    return false; // These are final states, no further transitions allowed
                default:
                    return false;
            }
        }
    }
}


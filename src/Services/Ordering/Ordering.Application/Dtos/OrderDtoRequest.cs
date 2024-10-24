using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos
{
    public record  OrderDtoRequest(Guid CustomerId,
                                  AddressDto ShippingAddress,
                                  string Payment,
                                  OrderStatus Status,
                                  string? CouponCode,
                                  List<OrderItemDto> OrderItems
                                  );
}

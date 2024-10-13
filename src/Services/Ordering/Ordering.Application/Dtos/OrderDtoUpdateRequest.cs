using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos
{
    public record OrderDtoUpdateRequest(Guid EntityId,
                                        Guid CustomerId,
                                        AddressDto ShippingAddress,
                                        PaymentDto Payment,
                                        OrderStatus Status);
}

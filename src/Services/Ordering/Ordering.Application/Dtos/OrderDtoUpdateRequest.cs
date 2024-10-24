using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos
{
    public record OrderDtoUpdateRequest(Guid EntityId,
                                        Guid CustomerId,
                                        AddressDto ShippingAddress,
                                        string Payment,
                                        OrderStatus Status);
}

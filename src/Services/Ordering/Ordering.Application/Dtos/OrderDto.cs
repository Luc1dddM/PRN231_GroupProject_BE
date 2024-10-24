using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos
{
    //we have developed these Dtos under the application layer
    //because we will map these these 2 objects when moving to presentation layer
    //that means when a request comes to the ordering API, we will map the DTO Dtos to entity objects in order
    //to perform our business logic
    public record OrderDto(Guid EntityId,
                           Guid CustomerId,
                           AddressDto ShippingAddress,
                           decimal TotalPrice,
                           string Payment,
                           string Status,
                           List<OrderItemDto> OrderItems,
                           string? CouponId);
}

namespace Ordering.Application.Extensions
{
    public static class OrderExtensions
    {
        public static IEnumerable<OrderDto> ToOrderDtoList(this IEnumerable<Order> orders)
        {
            return orders.Select(order => new OrderDto(
                EntityId: order.EntityId.Value,
                CustomerId: order.CustomerId.Value,
                TotalPrice: order.TotalPrice, // add
                ShippingAddress: new AddressDto(order.ShippingAddress.FirstName, 
                                                order.ShippingAddress.LastName, 
                                                order.ShippingAddress.Phone, 
                                                order.ShippingAddress.EmailAddress!, 
                                                order.ShippingAddress.AddressLine, 
                                                order.ShippingAddress.City, 
                                                order.ShippingAddress.District, 
                                                order.ShippingAddress.Ward),
                Payment: order.Payment,
                Status: order.Status.ToString(),
                CouponId: order.CouponId!,
                OrderItems: order.OrderItems.Select(oi => new OrderItemDto(oi.OrderId.Value, 
                                                                           oi.ProductId.Value,
                                                                           oi.ProductCategoryId,
                                                                           oi.Quantity, 
                                                                           oi.Price, 
                                                                           oi.Color)).ToList()
            ));
        }


        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto(
                EntityId: order.EntityId.Value,
                CustomerId: order.CustomerId.Value,
                TotalPrice: order.TotalPrice, // add total price
                ShippingAddress: new AddressDto(order.ShippingAddress.FirstName,
                                                order.ShippingAddress.LastName,
                                                order.ShippingAddress.Phone,
                                                order.ShippingAddress.EmailAddress!,
                                                order.ShippingAddress.AddressLine,
                                                order.ShippingAddress.City,
                                                order.ShippingAddress.District,
                                                order.ShippingAddress.Ward),
                Payment: order.Payment,
                Status: order.Status.ToString(),
                CouponId: order.CouponId!,
                OrderItems: order.OrderItems.Select(oi => new OrderItemDto(oi.OrderId.Value,
                                                                           oi.ProductId.Value,
                                                                           oi.ProductCategoryId,
                                                                           oi.Quantity,
                                                                           oi.Price,
                                                                           oi.Color)).ToList()
            );
        }

    }
}

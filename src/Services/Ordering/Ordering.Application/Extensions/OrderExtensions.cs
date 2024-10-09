namespace Ordering.Application.Extensions
{
    public static class OrderExtensions
    {
        public static IEnumerable<OrderDto> ToOrderDtoList(this IEnumerable<Order> orders)
        {
            return orders.Select(order => new OrderDto(
                Id: order.Id.Value,
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
                Payment: new PaymentDto(order.Payment.CardName!, 
                                        order.Payment.CardNumber, 
                                        order.Payment.Expiration, 
                                        order.Payment.CVV, 
                                        order.Payment.PaymentMethod),
                Status: order.Status,
                CouponId: order.CouponId!,
                OrderItems: order.OrderItems.Select(oi => new OrderItemDto(oi.OrderId.Value, 
                                                                           oi.ProductId.Value, 
                                                                           oi.Quantity, 
                                                                           oi.Price, 
                                                                           oi.Color)).ToList()
            ));
        }


    }
}

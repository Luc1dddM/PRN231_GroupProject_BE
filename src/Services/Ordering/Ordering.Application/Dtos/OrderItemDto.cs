namespace Ordering.Application.Dtos
{
    public record OrderItemDto(Guid OrderId, 
                               Guid ProductId,
                               string ProductCategoryId,
                               int Quantity, 
                               decimal Price,
                               string Color);
}

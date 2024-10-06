namespace Ordering.Application.Dtos
{
    public record PaymentDto(string CardName, 
                             string CardNumber, 
                             string Expiration, 
                             string Cvv, //CVV must be Cvv in order to Mapster to map to the entity, Mapster cannot map uppercase properties inside record type
                             string PaymentMethod);
}

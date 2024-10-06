namespace Ordering.Application.Dtos
{
    public record AddressDto(string FirstName, 
                             string LastName,
                             string Phone,
                             string EmailAddress, 
                             string AddressLine, 
                             string City, 
                             string District, 
                             string Ward);
}

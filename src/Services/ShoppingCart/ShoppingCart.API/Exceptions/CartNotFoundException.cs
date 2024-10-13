

namespace ShoppingCart.API.Exceptions;

public class CartNotFoundException : NotFoundException
{
    public CartNotFoundException(string userId) : base($"Cart of user with Id \"{userId}\" was not found")
    {

    }
}

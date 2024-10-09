using BuildingBlocks.Exceptions;

namespace Identity.Infrastructure.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string id) : base("User Not Found!", id)
        {

        }
    }
}

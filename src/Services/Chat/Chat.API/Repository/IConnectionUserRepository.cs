using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IConnectionUserRepository
    {
        public void Create(string UserId, string Name);
        public ConnectionUser GetUserById(string UserId);
        public List<ConnectionUser> GetCustomer();
    }
}

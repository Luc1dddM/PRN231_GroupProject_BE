using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IConnectionUserRepository
    {
        public void Create(ConnectionUser user);
        public List<ConnectionUser> GetEmployee();
        public ConnectionUser GetUserById(string UserId);
        public List<ConnectionUser> GetCustomer(); 

        public void Update(ConnectionUser user);
    }
}

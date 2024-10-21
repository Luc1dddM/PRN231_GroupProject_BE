using Chat.API.Model;

namespace Chat.API.Repository
{
    public class ConnectionUserRepository : IConnectionUserRepository
    {
        private readonly MyDbContext _context;

        public ConnectionUserRepository(MyDbContext myDbContext)
        {
            _context = myDbContext;
            
        }

        public void Create(ConnectionUser user)
        {
            try
            {
                _context.ConnectionUsers.Add(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public List<ConnectionUser> GetCustomer()
        {
            try
            {
                return _context.ConnectionUsers.Where(c => c.IsCustomer).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public List<ConnectionUser> GetEmployee()
        {
            try
            {
                return _context.ConnectionUsers.Where(c => !c.IsCustomer).ToList();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public ConnectionUser GetUserById(string UserId)
        {
            try
            {
                return _context.ConnectionUsers.FirstOrDefault(u => u.UserId.Equals(UserId));

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public void Update(ConnectionUser user)
        {
            try
            {
                var newUser = GetUserById(user.UserId);
                newUser.Name = user.Name;
                newUser.IsCustomer = user.IsCustomer;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

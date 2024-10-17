using Chat.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Chat.API.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly MyDbContext _context;
        public GroupRepository(MyDbContext myDbContext)
        {
            _context = myDbContext;
        }
        public void Create(string name)
        {
            try
            {
                var group = new Group
                {
                    GroupName = name
                };
                _context.Groups.Add(group);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<Group>> GetGroupByUserId(string userId)
        {
            try
            {
                return _context.Groups.Include(g => g.GroupMembers)
                    .Where(g => g.GroupMembers.Any(m => m.UserId.Equals(userId))).ToListAsync();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

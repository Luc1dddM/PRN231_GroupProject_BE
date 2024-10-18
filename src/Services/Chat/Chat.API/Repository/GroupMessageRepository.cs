
using Chat.API.Model;

namespace Chat.API.Repository
{
    public class GroupMessageRepository : IGroupMessageRepository
    {
        private readonly MyDbContext _context;
        public GroupMessageRepository(MyDbContext context)
        {
            _context = context;
        }
        public void Create(List<string> userId, string groupId, string? addBy)
        {
            try
            {
                foreach (var item in userId)
                {
                    var tmp = new GroupMember
                    {
                        GroupId = groupId,
                        UserId = item,
                        AddBy = addBy,
                        AddedDate = DateTime.Now
                    };
                    _context.Add(tmp);
                    _context.SaveChanges();
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

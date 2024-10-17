
using Chat.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Chat.API.Repository
{
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly MyDbContext _context;
        public GroupMemberRepository(MyDbContext context)
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

        public async Task<List<GroupMember>> GetGroupMemberByGroupId(string groupId)
        {
            try
            {
                return await _context.GroupMembers.Where(g => g.GroupId.Equals(groupId)).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

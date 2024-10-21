
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
        public void Create(List<ConnectionUser> user, string groupId, string? addBy)
        { 
            try
            {
                foreach (var item in user)
                {
                    var tmp = new GroupMember
                    {
                        GroupId = groupId,
                        UserId = item.UserId,
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

        public void OutGroup(string userId)
        {
            try
            {
                var groupRemove = _context.GroupMembers.Where(g => g.UserId.Equals(userId)).ToList();
                foreach (var item in groupRemove)
                {
                    _context.Remove(item);
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

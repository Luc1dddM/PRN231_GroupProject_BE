using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IGroupMemberRepository
    {
        public void Create(List<string> userId, string groupId, string? addBy);
        public Task<List<GroupMember>> GetGroupMemberByGroupId(string groupId);
    }
}

using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IGroupMemberRepository
    {
        public void Create(List<ConnectionUser> user, string groupId, string? addBy);
        public Task<List<GroupMember>> GetGroupMemberByGroupId(string groupId);
        public void OutGroup(string userId);
    }
}

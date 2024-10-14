

using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IGroupRepository
    {
        public void Create(string name);
        public List<Group> GetGroupByUserId(string userId);
    }
}

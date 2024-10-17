using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IMessageRepository
    {
        public void Create(string content, string senderId, string groupId);
        public Task<List<Message>> GetAllMessageByGroupId(string groupId);
        
    }
}

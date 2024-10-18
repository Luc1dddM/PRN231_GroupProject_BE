using Chat.API.Model;

namespace Chat.API.Repository
{
    public interface IMessageRepository
    {
        public void Create(Message message);
        public Task<List<Message>> GetAllMessageByGroupId(string groupId);
        
    }
}

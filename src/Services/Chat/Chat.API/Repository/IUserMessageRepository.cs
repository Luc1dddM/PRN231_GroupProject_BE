using Chat.API.Model.DTO;

namespace Chat.API.Repository
{
    public interface IUserMessageRepository
    {
        public void CreateUserMessageAsync(string messageId, string groupId);
        public void UpdateUserMessageAsync(string groupId, string receiverId);
        public List<UnReadNotifyDTO> CountUnReadMessage(string receiverId);
        public int CountTotalUnReadMessage(string receiverId);
    }
}

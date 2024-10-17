namespace Chat.API.Repository
{
    public interface IUserMessageRepository
    {
        public void CreateUserMessageAsync(string messageId, string groupId);
        public Task UpdateUserMessageAsync(string groupId, string receiverId);
        public int CountUnReadMessage(string receiverId, string groundId);
    }
}

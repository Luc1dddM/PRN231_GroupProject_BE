namespace Chat.API.Repository
{
    public interface IGroupMessageRepository
    {
        public void Create(List<string> userId, string groupId, string? addBy);
    }
}

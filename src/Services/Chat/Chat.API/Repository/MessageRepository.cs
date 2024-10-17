using Chat.API.Model;
using Microsoft.EntityFrameworkCore;

namespace Chat.API.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly MyDbContext _context;

        public MessageRepository(MyDbContext context)
        {
            _context = context;
        }
        public void Create(string content, string senderId, string groupId)
        {
            try
            {
                var message = new Message
                {
                    Content = content,
                    SenderId = senderId,
                    CreateAt = DateTime.Now,
                    GroupId = groupId,
                };
                _context.Messages.Add(message);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public Task<List<Message>> GetAllMessageByGroupId(string groupId)
        {
            try
            {
                return _context.Messages.Where(m => m.GroupId.Equals(groupId)).ToListAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


    }
}

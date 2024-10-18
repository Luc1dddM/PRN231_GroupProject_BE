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
        public void Create(Message message)
        {
            try
            {
                message.CreateAt = DateTime.Now;
                
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

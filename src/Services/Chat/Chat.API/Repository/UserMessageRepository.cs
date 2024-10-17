using Chat.API.Model;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Chat.API.Repository
{
    public class UserMessageRepository : IUserMessageRepository
    {
        private readonly MyDbContext _context;
        private readonly IGroupMemberRepository _groupMemberRepository;
        public UserMessageRepository(MyDbContext context
            ,IGroupMemberRepository groupMemberRepository)
        {
            _context = context;
            _groupMemberRepository = groupMemberRepository;
        }
        public int CountUnReadMessage(string receiverId, string groundId)
        {
            try
            {
                var count = _context.UserMessages.Include(p => p.Message)
                    .Where(u => u.ReceiverId.Equals(receiverId) 
                    && u.Message.GroupId.Equals(groundId) && !u.Status).Count();
                return count;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CreateUserMessageAsync(string messageId, string groupId)
        {
            try
            {
                var groupMember = _groupMemberRepository.GetGroupMemberByGroupId(groupId).Result;
                foreach (var item in groupMember)
                {
                    var userMessage = new UserMessage
                    {
                        MessageId = messageId,
                        ReceiverId = item.UserId,
                        Status = false
                    };
                    _context.UserMessages.Add(userMessage);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateUserMessageAsync(string groupId, string receiverId)
        {
            try
            {
                var usermessage =  _context.UserMessages.Include(p => p.Message)
                                                        .Where(p => p.Message.GroupId
                                                        .Equals(groupId)&&p.ReceiverId
                                                        .Equals(receiverId)&&!p.Status).ToList();
                foreach (var item in usermessage)
                {
                    item.Status = true;
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

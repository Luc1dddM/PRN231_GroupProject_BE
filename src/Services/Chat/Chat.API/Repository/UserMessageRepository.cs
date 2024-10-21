using Chat.API.Model;
using Chat.API.Model.DTO;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.EntityFrameworkCore;

namespace Chat.API.Repository
{
    public class UserMessageRepository : IUserMessageRepository
    {
        private readonly MyDbContext _context;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IGroupRepository _groupRepository;
        public UserMessageRepository(MyDbContext context
            ,IGroupMemberRepository groupMemberRepository
            ,IGroupRepository groupRepository)
        {
            _context = context;
            _groupMemberRepository = groupMemberRepository;
            _groupRepository = groupRepository;
        }

        public int CountTotalUnReadMessage(string receiverId)
        {
            try
            {
                var count = _context.UserMessages.Where(u => u.ReceiverId.Equals(receiverId) && !u.Status).Count();
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<UnReadNotifyDTO> CountUnReadMessage(string receiverId)
        {
            try
            {
                var message = _context.UserMessages.Include(p => p.Message)
                    .Where(u => u.ReceiverId.Equals(receiverId) && !u.Status).AsQueryable();
                var group = _groupRepository.GetGroupByUserId(receiverId).Result;
                var result = new List<UnReadNotifyDTO>();
                foreach (var item in group)
                {
                    var count = message.Where(m => m.Message.GroupId.Equals(item.GroupId)).Count();
                    var tmp = new UnReadNotifyDTO
                    {
                        groupId = item.GroupId,
                        Count = count
                    };
                    result.Add(tmp);    
                }



                return result;
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

        public void UpdateUserMessageAsync(string groupId, string receiverId)
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
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
    }
}

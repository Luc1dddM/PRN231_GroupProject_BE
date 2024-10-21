using BuildingBlocks.Exceptions;
using Chat.API.Model;
using Chat.API.Repository;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace Chat.API.Hubs
{
    public class ChatHub : Hub
    {

        private readonly IDictionary<string, List<string>> _groupMember; 
        private readonly IDictionary<string, ConnectionUser> _userConnected;

        private readonly IConnectionUserRepository _connectionUserRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserMessageRepository _userMessageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ChatHub(IConnectionUserRepository connectionUserRepository
            , IGroupRepository groupRepository
            , IMessageRepository messageRepository
            , IUserMessageRepository userMessageRepository
            , IHttpContextAccessor httpContextAccessor
            , IDictionary<string, List<string>> groupMember
            , IDictionary<string, ConnectionUser> userConnected)
        {
            _httpContextAccessor = httpContextAccessor;
            _connectionUserRepository = connectionUserRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _userMessageRepository = userMessageRepository;
            _groupMember = groupMember;
            _userConnected = userConnected;

        }


        public async Task OnConnected(string userId)
        {
            var user = GetConnectionUser(userId);
            if(Context.ConnectionId != null)
            {
                _userConnected[Context.ConnectionId] = user;

            }
        }



        public async Task JoinRoom(string userId, string groupId)
        {

            
                await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
                if (!_groupMember.ContainsKey(groupId))
                {
                    _groupMember[groupId] = new List<string>();
                }
                if (!_groupMember[groupId].Contains(Context.ConnectionId))
                {
                    _groupMember[groupId].Add(Context.ConnectionId);
                }

            foreach (var item in _groupMember[groupId])
            {
                if (item.Equals(Context.ConnectionId))
                {
                    _userMessageRepository.UpdateUserMessageAsync(groupId, _userConnected[item].UserId);
                }
            }


        }

        public async Task SendMessage(string message, string groupId, string userId)
        {

            try
            {
                var user = GetConnectionUser(userId);
                var newMessage = new Model.Message
                {
                    Content = message,
                    GroupId = groupId,
                    SenderId = userId,

                };
                _messageRepository.Create(newMessage);
                _userMessageRepository.CreateUserMessageAsync(newMessage.MessageId, groupId);
                foreach (var item in _groupMember[groupId])
                {

                    _userMessageRepository.UpdateUserMessageAsync(groupId, _userConnected[item].UserId);
                }
                var totalNotify = _userMessageRepository.CountTotalUnReadMessage(_userConnected[Context.ConnectionId].UserId);
                await Clients.All.SendAsync("ReceiveNotifyTotal", totalNotify);
                await Clients.Group(groupId).SendAsync("ReceiveMessage", user.UserId, user.Name, message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            


        }




        //we do not need to explicitly call on the front-end for this OnDisconnectedAsync since we over the method of SignalR
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            foreach (var item in _groupMember.Values)
            {
                if (item.Contains(Context.ConnectionId))
                {
                    item.Remove(Context.ConnectionId);
                }
            }

            _userConnected.Remove(Context.ConnectionId);
            

            return base.OnDisconnectedAsync(exception);
        }


        public Task SendUsersConnected(string room)
        {
            var users = _connectionUserRepository.GetCustomer();

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }

        public ConnectionUser GetConnectionUser(string userId)
        {
            var user = _connectionUserRepository.GetUserById(userId);
            return user;
        }

    }
}

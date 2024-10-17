using BuildingBlocks.Exceptions;
using Chat.API.Model;
using Chat.API.Repository;
using Microsoft.AspNetCore.SignalR;

namespace Chat.API.Hubs
{
    public class ChatHub : Hub
    {
        private readonly Dictionary<string, List<ConnectionUser>> groupMember = new Dictionary<string, List<ConnectionUser>>();
        private readonly IConnectionUserRepository _connectionUserRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IUserMessageRepository _userMessageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public ChatHub(IConnectionUserRepository connectionUserRepository
            , IGroupRepository groupRepository
            , IMessageRepository messageRepository
            , IUserMessageRepository userMessageRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _connectionUserRepository = connectionUserRepository;
            _groupRepository = groupRepository;
            _messageRepository = messageRepository;
            _userMessageRepository = userMessageRepository;

        }

        public async Task JoinRoom()
        {
            var userId = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            var user = GetConnectionUser();

            var listRoom = await _groupRepository.GetGroupByUserId(userId);
            foreach (var room in listRoom)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.GroupId);
                if (!groupMember.ContainsKey(room.GroupId))
                {
                    groupMember[room.GroupId] = new List<ConnectionUser>();
                }
                groupMember[room.GroupId].Add(user);
            }

        }

        public async Task SendMessage(string message, string groupId)
        {

            var user = GetConnectionUser();
            _messageRepository.Create(message, user.UserId, groupId);
            foreach (var item in groupMember[groupId])
            {
                _userMessageRepository.UpdateUserMessageAsync(groupId, item.UserId);
            }
            await Clients.Group(groupId).SendAsync("ReceiveMessage", user.Name, message);


        }




        //we do not need to explicitly call on the front-end for this OnDisconnectedAsync since we over the method of SignalR
        public Task OnDisconnectedAsync(Exception? exception, string groupId)
        {
            var user = GetConnectionUser();
            groupMember[groupId].Remove(user);

            return base.OnDisconnectedAsync(exception);
        }


        public Task SendUsersConnected(string room)
        {
            var users = _connectionUserRepository.GetCustomer();

            return Clients.Group(room).SendAsync("UsersInRoom", users);
        }

        public ConnectionUser GetConnectionUser()
        {
            var userId = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId)) throw new BadRequestException("User Id Is Null");
            var user = _connectionUserRepository.GetUserById(userId);
            return user;
        }

    }
}

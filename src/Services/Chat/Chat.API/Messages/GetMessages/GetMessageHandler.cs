using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Chat.API.Model.DTO;
using Chat.API.Repository;
using Mapster;

namespace Chat.API.Messages.GetMessages
{
    public record GetMessageQuery(string groupId) : IQuery<GetMessageResult>;
    public record GetMessageResult(BaseResponse<List<MessageDTO>> listMessage);

    internal class GetMessagesQueryHandler : IQueryHandler<GetMessageQuery, GetMessageResult>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IConnectionUserRepository _connectionUserRepository;

        public GetMessagesQueryHandler(IMessageRepository messageRepository
            , IConnectionUserRepository connectionUserRepository)
        {
            _messageRepository = messageRepository;
            _connectionUserRepository = connectionUserRepository;
        }

        public async Task<GetMessageResult> Handle(GetMessageQuery query, CancellationToken cancellationToken)
        {
            var messages = await _messageRepository.GetAllMessageByGroupId(query.groupId);

            var result = MappingMessageToMessageDTO(messages);
            return new GetMessageResult(new BaseResponse<List<MessageDTO>>(result));
        }


        private List<MessageDTO> MappingMessageToMessageDTO(List<Model.Message> list)
        {
            var listDTO = new List<MessageDTO>();
            foreach (var item in list)
            {
                var message = new MessageDTO
                {
                    Content = item.Content,
                    SenderId = item.SenderId,
                    SenderName = _connectionUserRepository.GetUserById(item.SenderId).Name
                };
                listDTO.Add(message);
            }
            return listDTO;
        }

    }
}

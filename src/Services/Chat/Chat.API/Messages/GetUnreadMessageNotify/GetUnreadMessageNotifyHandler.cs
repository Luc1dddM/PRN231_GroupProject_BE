using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Chat.API.Model.DTO;
using Chat.API.Repository;

namespace Chat.API.Messages.GetUnreadMessageNotify
{
    public record GetUnreadMessageNotifyQuery():IQuery<GetUnreadMessageNotifyResult>;
    public record GetUnreadMessageNotifyResult(BaseResponse<List<UnReadNotifyDTO>> result);

    internal class GetUnreadMessageNotifyHandler : IQueryHandler<GetUnreadMessageNotifyQuery, GetUnreadMessageNotifyResult>
    {
        private readonly IUserMessageRepository _userMessageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetUnreadMessageNotifyHandler(IUserMessageRepository userMessageRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _userMessageRepository = userMessageRepository;
        }
        public async Task<GetUnreadMessageNotifyResult> Handle(GetUnreadMessageNotifyQuery request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId)) throw new BadRequestException("User Id Is Null");
            var result = _userMessageRepository.CountUnReadMessage(userId);
            return new GetUnreadMessageNotifyResult(new BaseResponse<List<UnReadNotifyDTO>>(result, "Get Unread Message Count Successfuly"));
        }
    }
}

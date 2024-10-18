using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Chat.API.Repository;

namespace Chat.API.Messages.TotalNotify
{

    public record GetTotalNotifyQuery(string userId) : IQuery<GetTotalNotifyResult>;
    public record GetTotalNotifyResult(BaseResponse<int> result);

    internal class GetTotalNotifyHandler : IQueryHandler<GetTotalNotifyQuery, GetTotalNotifyResult>
    {
        private readonly IUserMessageRepository _userMessageRepository;
        private readonly IHttpContextAccessor _contextAccessor;

        public GetTotalNotifyHandler(IUserMessageRepository userMessageRepository
            , IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            _userMessageRepository = userMessageRepository;
        }

        public async Task<GetTotalNotifyResult> Handle(GetTotalNotifyQuery request, CancellationToken cancellationToken)
        {


            var result = _userMessageRepository.CountTotalUnReadMessage(request.userId);

            return new GetTotalNotifyResult(new BaseResponse<int>(result, "Get Notify successfuly"));
        }
    }
}

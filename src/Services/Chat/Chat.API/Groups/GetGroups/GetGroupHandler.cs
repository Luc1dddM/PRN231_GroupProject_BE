using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Chat.API.Exceptions;
using Chat.API.Model.DTO;
using Chat.API.Repository;
using Mapster;
using System.Text.RegularExpressions;

namespace Chat.API.Groups.GetGroups
{

    public record GetGroupQuery() : IQuery<GetGroupResult>;
    public record GetGroupResult(BaseResponse<List<GroupDTO>> result);

    internal class GetGroupQueryHandler:IQueryHandler<GetGroupQuery, GetGroupResult>
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public GetGroupQueryHandler(IGroupRepository groupRepository, IHttpContextAccessor httpContextAccessor)
        {
            _groupRepository = groupRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetGroupResult> Handle(GetGroupQuery query, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.Request.Headers["UserId"].ToString();
            if (string.IsNullOrEmpty(userId)) throw new BadRequestException("User Id Is Null");
            var group = await _groupRepository.GetGroupByUserId(userId);
            var result = group.Adapt<List<GroupDTO>>();
            return new GetGroupResult(new BaseResponse<List<GroupDTO>>(result,"Get groups successfuly"));
        }
    }

}

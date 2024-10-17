using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Chat.API.Exceptions;
using Chat.API.Model.DTO;
using Chat.API.Repository;
using Mapster;
using System.Text.RegularExpressions;

namespace Chat.API.Groups.GetGroups
{

    public record GetGroupQuery(string userId) : IQuery<GetGroupResult>;
    public record GetGroupResult(BaseResponse<List<GroupDTO>> result);

    internal class GetGroupQueryHandler:IQueryHandler<GetGroupQuery, GetGroupResult>
    {
        private readonly IGroupRepository _groupRepository;

        public GetGroupQueryHandler(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<GetGroupResult> Handle(GetGroupQuery query, CancellationToken cancellationToken)
        {
            var group = await _groupRepository.GetGroupByUserId(query.userId);
            var result = group.Adapt<List<GroupDTO>>();
            return new GetGroupResult(new BaseResponse<List<GroupDTO>>(result,"Get groups successfuly"));
        }
    }

}

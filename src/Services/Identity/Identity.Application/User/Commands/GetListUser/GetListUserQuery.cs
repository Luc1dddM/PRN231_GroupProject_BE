using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Dtos;

namespace Identity.Application.User.Commands.GetListUser
{
    public record GetListUsersQuery(GetListUserParamsDto parameters) : IQuery<GetListUsersResponse>;
    public record GetListUsersResponse(BaseResponse<PaginatedList<UserDto>> response);
}

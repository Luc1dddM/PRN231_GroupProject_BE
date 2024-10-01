using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;

namespace Identity.Application.User.Commands.GetListUser
{
    public record GetListUsersQuery(GetListUserParamsDto parameters) : IQuery<GetListUsersResponse>;
    public record GetListUsersResponse(BaseResponse<PaginatedList<Domain.Entities.User>> response);
}

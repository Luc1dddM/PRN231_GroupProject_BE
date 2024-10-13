using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Dtos;

namespace Identity.Application.User.Commands.GetUserById
{
    public record GetUserByIdQuery(string id) : IQuery<GetUserByIdResponse>;
    public record GetUserByIdResponse(BaseResponse<UserDto> response);
}

using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Dtos;

namespace Identity.Application.User.Commands.Updateuser
{
    public record UpdateUserCommand(string id, UpdateUserDto command) : ICommand<UpdateUserResponse>;

    public record UpdateUserResponse(BaseResponse<bool> response);
}

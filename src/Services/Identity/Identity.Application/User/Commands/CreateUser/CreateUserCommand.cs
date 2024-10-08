using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.User.Dtos;

namespace Identity.Application.User.Commands.CreateUser
{
    public record CreateUserCommand(CreateNewUserDto Dto) : ICommand<CreateUserResponse>;

    public record CreateUserResponse(BaseResponse<UserDto> response);
}

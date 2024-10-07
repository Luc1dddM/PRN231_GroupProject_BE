using BuildingBlocks.CQRS;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.CreateUser
{
    public record CreateUserCommand(CreateNewUserDto Dto) : ICommand<CreateUserResponse>;

    public record CreateUserResponse(BaseResponse<UserDto> response);
}

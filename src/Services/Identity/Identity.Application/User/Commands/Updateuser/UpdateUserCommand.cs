using BuildingBlocks.CQRS;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.Updateuser
{
    public record UpdateUserCommand(string id, UpdateUserDto command) : ICommand<UpdateUserResponse>;

    public record UpdateUserResponse(BaseResponse<bool> response);
}

using BuildingBlocks.CQRS;
using Identity.Application.User.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.GetUserById
{
    public record GetUserByIdQuery(string id) : IQuery<GetUserByIdResponse>;
    public record GetUserByIdResponse(BaseResponse<UserDto> response);
}

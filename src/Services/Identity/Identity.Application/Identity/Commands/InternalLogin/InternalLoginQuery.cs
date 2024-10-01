using BuildingBlocks.CQRS;
using Identity.Application.Identity.Dtos;
using Identity.Application.Utils;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.Application.Identity.Commands.InternalLogin
{
    public record InternalLoginQuery(string UserName, string Password) : IQuery<InternalLoginResult>;
    public record InternalLoginResult(BaseResponse<JwtResponseVM> response);
}

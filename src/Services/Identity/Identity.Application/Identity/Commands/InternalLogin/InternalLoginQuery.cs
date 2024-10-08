using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.Identity.Dtos;

namespace Identity.Application.Identity.Commands.InternalLogin
{
    public record InternalLoginQuery(string UserName, string Password) : IQuery<InternalLoginResult>;
    public record InternalLoginResult(BaseResponse<JwtModelVM> response);
}

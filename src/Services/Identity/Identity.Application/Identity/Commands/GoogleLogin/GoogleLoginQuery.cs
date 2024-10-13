using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.Identity.Dtos;

namespace Identity.Application.Identity.Commands.GoogleLogin
{
    public record GoogleLoginQuery(string Token) : IQuery<GoogleLoginResponse>;

    public record GoogleLoginResponse(BaseResponse<LoginReponseDto> response);
}

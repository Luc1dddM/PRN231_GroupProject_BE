using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.Identity.Dtos;

namespace Identity.Application.Identity.Commands.RenewToken
{
    public record RenewTokenCommand(JwtModelVM request) : ICommand<RenewTokenResponse>;
    public record RenewTokenResponse(BaseResponse<JwtModelVM> response);
}

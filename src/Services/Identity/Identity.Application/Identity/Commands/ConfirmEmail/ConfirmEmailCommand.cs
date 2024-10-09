using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace Identity.Application.Identity.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string UserId, string Token) : ICommand<ConfirmEmailResponse>;

    public record ConfirmEmailResponse(BaseResponse<string> result);
}

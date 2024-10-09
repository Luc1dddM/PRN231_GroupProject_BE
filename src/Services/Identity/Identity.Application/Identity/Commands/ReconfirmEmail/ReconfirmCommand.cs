using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace Identity.Application.Identity.Commands.ReconfirmEmail
{
    public record ReconfirmCommand(string EmailAddress) : ICommand<ReconfirmResponse>;

    public record ReconfirmResponse(BaseResponse<string> response);
}

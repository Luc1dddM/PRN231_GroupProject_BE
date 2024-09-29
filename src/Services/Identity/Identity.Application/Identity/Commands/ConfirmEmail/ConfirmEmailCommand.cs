using BuildingBlocks.CQRS;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.ConfirmEmail
{
    public record ConfirmEmailCommand(string UserId, string Token) : ICommand<ConfirmEmailResponse>;

    public record ConfirmEmailResponse(BaseResponse<string> result);
}

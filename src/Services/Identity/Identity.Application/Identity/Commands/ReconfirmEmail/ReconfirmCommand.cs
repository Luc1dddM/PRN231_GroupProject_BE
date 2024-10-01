using BuildingBlocks.CQRS;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.ReconfirmEmail
{
    public record ReconfirmCommand(string EmailAddress) : ICommand<ReconfirmResponse>;

    public record ReconfirmResponse(BaseResponse<string> response);
}

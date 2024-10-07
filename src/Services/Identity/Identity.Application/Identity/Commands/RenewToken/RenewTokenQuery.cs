using BuildingBlocks.CQRS;
using Identity.Application.Identity.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.RenewToken
{
    public record RenewTokenCommand(JwtModelVM request) : ICommand<RenewTokenResponse>;
    public record RenewTokenResponse(BaseResponse<JwtModelVM> response);
}

using BuildingBlocks.CQRS;
using Identity.Application.DTOs;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.GoogleLogin
{
    public record GoogleLoginQuery(GoogleSignInVM request) : IQuery<GoogleLoginResponse>;

    public record GoogleLoginResponse(BaseResponse<JwtResponseVM> response);
}

using BuildingBlocks.CQRS;
using Identity.Application.DTOs;
using Identity.Application.Utils;
using Microsoft.AspNetCore.Identity.Data;

namespace Identity.Application.Identity.Commands.InternalLogin
{
   public record InternalLoginQuery(LoginRequestDto request) : IQuery<InternalLoginResult>;
   public record InternalLoginResult(BaseResponse<JwtResponseVM> response);
}

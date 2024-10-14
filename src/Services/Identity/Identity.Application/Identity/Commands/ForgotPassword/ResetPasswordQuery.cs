using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.Identity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.ResetPassword
{
    public record ResetPasswordQuery(string email) : IQuery<ResetPasswordResponse>;
    
    public record ResetPasswordResponse(BaseResponse<string> response);
}

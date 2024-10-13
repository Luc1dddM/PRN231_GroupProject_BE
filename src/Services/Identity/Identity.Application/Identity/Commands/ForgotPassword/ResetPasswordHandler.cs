using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.ResetPassword
{
    public class ResetPasswordHandler : IQueryHandler<ResetPasswordQuery, ResetPasswordResponse>
    {
        public IAuthService _authSerivce;
        public ResetPasswordHandler(IAuthService authSerivce)
        {
            _authSerivce = authSerivce;
        }
        public async Task<ResetPasswordResponse> Handle(ResetPasswordQuery request, CancellationToken cancellationToken)
        {
            var response = await _authSerivce.ForgotPassword(request.email);
            return new ResetPasswordResponse(response);
        }
    }
}

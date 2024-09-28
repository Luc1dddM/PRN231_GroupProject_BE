using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.InternalLogin
{
    public class InternalLoginHandler : IQueryHandler<InternalLoginQuery, InternalLoginResult>
    {
        IAuthService _authService;
        public InternalLoginHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<InternalLoginResult> Handle(InternalLoginQuery request, CancellationToken cancellationToken)
        {
            var result = await _authService.Login(request.request);
            return new InternalLoginResult(result);
        }
    }
}

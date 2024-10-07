using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;
using Identity.Application.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.RenewToken
{
    public class RenewTokenHandler : ICommandHandler<RenewTokenCommand, RenewTokenResponse>
    {
        private readonly IAuthService _authService;
        public RenewTokenHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RenewTokenResponse> Handle(RenewTokenCommand request, CancellationToken cancellationToken)
        {
            var result =await _authService.RenewToken(request.request);
            return new RenewTokenResponse(result);
        }
    }
}

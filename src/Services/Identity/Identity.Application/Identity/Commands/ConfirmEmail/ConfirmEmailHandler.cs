using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.ConfirmEmail
{
    public class ConfirmEmailHandler : ICommandHandler<ConfirmEmailCommand, ConfirmEmailResponse>
    {
        private IAuthService _authService;
        public ConfirmEmailHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<ConfirmEmailResponse> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            var response = await _authService.ConfirmEmail(request.UserId, request.Token);
            return new ConfirmEmailResponse(response);
        }
    }
}

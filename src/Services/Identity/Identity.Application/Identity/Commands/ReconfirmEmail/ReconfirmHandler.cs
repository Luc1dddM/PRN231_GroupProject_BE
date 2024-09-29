using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.ReconfirmEmail
{
    public class ReconfirmHandler : ICommandHandler<ReconfirmCommand, ReconfirmResponse>
    {

        IAuthService _authService;
        public ReconfirmHandler(IAuthService authService)
        {
            _authService = authService;
        }
        public async Task<ReconfirmResponse> Handle(ReconfirmCommand request, CancellationToken cancellationToken)
        {
            var response = await _authService.ReConfirmEmail(request.command);

            return new ReconfirmResponse(response);
        }
    }
}

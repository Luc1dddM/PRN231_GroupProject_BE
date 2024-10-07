using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Identity.Commands.GoogleLogin
{
    public class GoogleLoginHandler : IQueryHandler<GoogleLoginQuery, GoogleLoginResponse>
    {
        private IAuthService _authService;
        public GoogleLoginHandler(IAuthService authService) { 
            _authService = authService;
        }
        public async Task<GoogleLoginResponse> Handle(GoogleLoginQuery request, CancellationToken cancellationToken)
        {
            var response = await _authService.SignInWithGoogle(request.Token);
            return new GoogleLoginResponse(response);

        }
    }
}

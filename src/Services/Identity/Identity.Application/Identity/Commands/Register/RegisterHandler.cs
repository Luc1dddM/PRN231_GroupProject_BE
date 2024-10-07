using BuildingBlocks.CQRS;
using Identity.Application.Identity.Interfaces;

namespace Identity.Application.Identity.Commands.Register
{
    public class RegisterHandler : ICommandHandler<RegisterCommand, RegisterResult>
    {
        private IAuthService _authService;
        public RegisterHandler(IAuthService authService) { 
            _authService = authService;
        }
        public async Task<RegisterResult> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
           var usrDto = await _authService.Register(request.Email, request.Name, request.Phonenumber, request.Password);
            return new RegisterResult(usrDto);
        }
    }
}

using BuildingBlocks.CQRS;
using Identity.Application.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.CreateUser
{
    public class CreateUserHandler : ICommandHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserService _userService;
        public CreateUserHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<CreateUserResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.CreateNewUser(request.Dto);
            return new CreateUserResponse(result);
        }
    }
}

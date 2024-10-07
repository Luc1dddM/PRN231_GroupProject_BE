using BuildingBlocks.CQRS;
using Identity.Application.User.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.User.Commands.Updateuser
{
    public class UpdateUserHandler : ICommandHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IUserService _service;
        public UpdateUserHandler(IUserService service)
        {
            _service = service; 
        }
        public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateUser(request.id, request.command);
            return new UpdateUserResponse(result);
        }
    }
}

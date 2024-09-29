using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.UpdatePermission
{
    public class UpdatePermissionHandler : ICommandHandler<UpdatePermissionCommand, UpdatePermissionResponse>
    {
        private readonly IRolePermissionService _service;

        public UpdatePermissionHandler(IRolePermissionService service)
        {
            _service = service;
        }
        public async Task<UpdatePermissionResponse> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
           var response = await _service.UpdatePermission(request.command);
           return new UpdatePermissionResponse(response);
        }
    }
}

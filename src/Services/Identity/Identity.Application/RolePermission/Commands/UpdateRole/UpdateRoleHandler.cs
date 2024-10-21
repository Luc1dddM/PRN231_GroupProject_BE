using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.UpdateRole
{
    public class UpdateRoleHandler : ICommandHandler<UpdateRoleCommand, UpdateRoleReponse>
    {
        private readonly IRolePermissionService _service;
        public UpdateRoleHandler(IRolePermissionService service)
        {
            _service = service;
        }

        public async Task<UpdateRoleReponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateRole(request.id, request.name, request.permissions);
            return new UpdateRoleReponse(result);

        }
    }
}

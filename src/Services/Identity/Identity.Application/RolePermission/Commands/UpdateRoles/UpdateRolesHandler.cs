using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Interfaces;

namespace Identity.Application.RolePermission.Commands.UpdateRoles
{
    public class UpdateRolesHandler : ICommandHandler<UpdateRolesCommand, UpdateRoleResponse>
    {
        private readonly IRolePermissionService _service;
        public UpdateRolesHandler(IRolePermissionService service)
        {
            _service = service;
        }

        public async Task<UpdateRoleResponse> Handle(UpdateRolesCommand request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateRoles(request.UserId, request.Roles);
            return new UpdateRoleResponse(result);
        }
    }
}

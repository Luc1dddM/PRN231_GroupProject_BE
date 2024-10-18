using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Interfaces;

namespace Identity.Application.RolePermission.Commands.UpdateRoles
{
    public class UpdateUserRolesHandler : ICommandHandler<UpdateUserRolesCommand, UpdateUserRoleResponse>
    {
        private readonly IRolePermissionService _service;
        public UpdateUserRolesHandler(IRolePermissionService service)
        {
            _service = service;
        }

        public async Task<UpdateUserRoleResponse> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var result = await _service.UpdateUserRoles(request.UserId, request.Roles);
            return new UpdateUserRoleResponse(result);
        }
    }
}

using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace Identity.Application.RolePermission.Commands.UpdateRoles
{
    public record UpdateRolesCommand(string UserId, List<string>? Roles) : ICommand<UpdateRoleResponse>;

    public record UpdateRoleResponse(BaseResponse<bool> response);
}

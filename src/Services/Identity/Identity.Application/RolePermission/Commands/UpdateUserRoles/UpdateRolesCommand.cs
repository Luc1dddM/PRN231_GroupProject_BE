using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace Identity.Application.RolePermission.Commands.UpdateRoles
{
    public record UpdateUserRolesCommand(string UserId, List<string>? Roles) : ICommand<UpdateUserRoleResponse>;

    public record UpdateUserRoleResponse(BaseResponse<bool> response);
}

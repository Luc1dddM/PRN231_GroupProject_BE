using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace Identity.Application.RolePermission.Commands.UpdatePermission
{
    public record UpdatePermissionCommand(string Role, List<int>? PermissionIds) : ICommand<UpdatePermissionResponse>;

    public record UpdatePermissionResponse(BaseResponse<bool> response);
}

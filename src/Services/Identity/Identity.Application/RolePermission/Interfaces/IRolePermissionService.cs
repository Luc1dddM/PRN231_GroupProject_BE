using BuildingBlocks.Models;

namespace Identity.Application.RolePermission.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<string>> GetPermissionsAsync(string UserId);

        Task<BaseResponse<bool>> UpdatePermission(string Role, List<int>? PermissionIds);

        Task<BaseResponse<bool>> UpdateRoles(string UserId, List<string>? Roles);

        Task<BaseResponse<bool>> AddNewRole(string Name);
    }
}

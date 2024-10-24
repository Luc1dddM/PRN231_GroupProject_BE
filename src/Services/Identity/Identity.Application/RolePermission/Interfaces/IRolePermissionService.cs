using BuildingBlocks.Models;
using Identity.Application.RolePermission.Dtos;

namespace Identity.Application.RolePermission.Interfaces
{
    public interface IRolePermissionService
    {
        Task<List<string>> GetPermissionsAsync(string UserId);

        Task<BaseResponse<bool>> UpdatePermission(string Role, List<int>? PermissionIds);

        Task<BaseResponse<bool>> UpdateUserRoles(string UserId, List<string>? Roles);

        Task<BaseResponse<RoleDto>> AddNewRole(string Name, string[] permissions);
        Task<BaseResponse<RoleDto>> UpdateRole(string Id, string Name, string[] permissions);

        Task<BaseResponse<List<RoleDto>>> GetListRole();
        Task<BaseResponse<List<PermissionManagerDto>>> GetListPermissions();

    }
}

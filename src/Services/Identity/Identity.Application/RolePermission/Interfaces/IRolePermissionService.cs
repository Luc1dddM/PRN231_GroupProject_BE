using Identity.Application.RolePermission.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Interfaces
{
    public interface IRolePermissionService
    {
        //Welcome to hell
        Task<List<string>> GetPermissionsAsync(string UserId);

        Task<BaseResponse<bool>> UpdatePermission(string Role, List<int>? PermissionIds);

        Task<BaseResponse<bool>> UpdateRoles(string UserId, List<string>? Roles);

        Task<BaseResponse<bool>> AddNewRole(string Name, string UserId);
    }
}

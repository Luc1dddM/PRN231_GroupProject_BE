using Identity.Application.Dtos;
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
        Task<List<string>> GetPermissionsAsync(string userId);

        Task<BaseResponse<bool>> UpdatePermission(UpdatePermissionDto updateUserDto);

        Task<BaseResponse<bool>> AddNewRole(string name, string userId);
    }
}

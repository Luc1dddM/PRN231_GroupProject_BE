using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.UpdatePermission
{
    public record UpdatePermissionCommand(string Role, List<int>? PermissionIds) : ICommand<UpdatePermissionResponse>;

    public record UpdatePermissionResponse(BaseResponse<bool> response);
}

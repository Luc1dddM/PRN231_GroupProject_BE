using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.UpdateRoles
{
    public record UpdateRolesCommand(string UserId, List<string>? Roles) : ICommand<UpdateRoleResponse>;

    public record UpdateRoleResponse(BaseResponse<bool> response);
}

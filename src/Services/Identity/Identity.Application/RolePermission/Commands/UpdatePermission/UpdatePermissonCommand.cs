using BuildingBlocks.CQRS;
using Identity.Application.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.UpdatePermission
{
    public record UpdatePermissionCommand(UpdatePermissionDto command) : ICommand<UpdatePermissionResponse>;

    public record UpdatePermissionResponse(BaseResponse<bool> response);
}

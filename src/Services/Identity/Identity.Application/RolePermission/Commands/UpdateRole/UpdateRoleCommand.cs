using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.RolePermission.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.UpdateRole
{
    public record UpdateRoleCommand(string id, string name, string[] permissions) : ICommand<UpdateRoleReponse>;

    public record UpdateRoleReponse(BaseResponse<RoleDto> response);
}

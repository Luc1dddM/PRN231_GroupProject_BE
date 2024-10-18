using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.RolePermission.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.GetRoles
{
    public record GetRolesQuery() : IQuery<GetRolesResponse>;

    public record GetRolesResponse(BaseResponse<List<RoleDto>> result);


}

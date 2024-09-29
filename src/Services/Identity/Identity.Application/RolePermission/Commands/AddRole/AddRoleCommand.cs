using BuildingBlocks.CQRS;
using Identity.Application.Dtos;
using Identity.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.AddRole
{
    public record AddRoleCommand(string name, string CreatedBy) : ICommand<AddRoleReponse>
    {
    }

    public record AddRoleReponse(BaseResponse<bool> response);
}

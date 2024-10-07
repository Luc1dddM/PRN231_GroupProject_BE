using BuildingBlocks.CQRS;
using Identity.Application.Utils;

namespace Identity.Application.RolePermission.Commands.AddRole
{
    public record AddRoleCommand(string Name, string CreatedBy) : ICommand<AddRoleReponse>
    {
    }

    public record AddRoleReponse(BaseResponse<bool> response);
}

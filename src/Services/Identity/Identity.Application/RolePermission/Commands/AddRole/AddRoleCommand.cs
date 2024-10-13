using BuildingBlocks.CQRS;
using BuildingBlocks.Models;

namespace Identity.Application.RolePermission.Commands.AddRole
{
    public record AddRoleCommand(string Name) : ICommand<AddRoleReponse>
    {
    }

    public record AddRoleReponse(BaseResponse<bool> response);
}

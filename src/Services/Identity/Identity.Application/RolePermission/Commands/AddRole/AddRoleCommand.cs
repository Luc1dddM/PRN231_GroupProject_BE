using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.RolePermission.Dtos;

namespace Identity.Application.RolePermission.Commands.AddRole
{
    public record AddRoleCommand(string Name, string[] permissions) : ICommand<AddRoleReponse>
    {
    }

    public record AddRoleReponse(BaseResponse<RoleDto> response);
}

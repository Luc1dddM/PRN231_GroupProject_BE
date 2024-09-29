using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.AddRole
{
    public class AddRoleHandler : ICommandHandler<AddRoleCommand, AddRoleReponse>
    {
        private readonly IRolePermissionService _rolePermissionService;
        public AddRoleHandler(IRolePermissionService rolePermissionService) { 
            _rolePermissionService = rolePermissionService;
        }
        public async Task<AddRoleReponse> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var response = await _rolePermissionService.AddNewRole(request.name, request.CreatedBy);
            return new AddRoleReponse(response);
        }
    }
}

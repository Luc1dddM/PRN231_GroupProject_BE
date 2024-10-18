using BuildingBlocks.CQRS;
using Identity.Application.RolePermission.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.GetRoles
{
    public class GetRolesHandler : IQueryHandler<GetRolesQuery, GetRolesResponse>
    {
        private readonly IRolePermissionService _service;
        public GetRolesHandler(IRolePermissionService service)
        {
            _service = service;
        }

        public async Task<GetRolesResponse> Handle(GetRolesQuery request, CancellationToken cancellationToken)
        {
            var result = await _service.GetListRole();
            return new GetRolesResponse(result);
        }
    }
}

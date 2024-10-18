using BuildingBlocks.CQRS;
using BuildingBlocks.Models;
using Identity.Application.RolePermission.Dtos;
using Identity.Application.RolePermission.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Commands.GetListPermissions
{
    public class GetListPermissionHandler : IQueryHandler<GetPermissionsQuery, GetPermissionsResponse>
    {
        private readonly IRolePermissionService _service;
        public GetListPermissionHandler(IRolePermissionService service)
        {
            _service = service;
        }

        public async Task<GetPermissionsResponse> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
        {
            var result = await _service.GetListPermissions();
            return new GetPermissionsResponse(result);
        }
    }
}

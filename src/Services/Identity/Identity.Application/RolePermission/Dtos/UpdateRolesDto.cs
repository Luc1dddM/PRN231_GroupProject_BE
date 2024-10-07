using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Dtos
{
    public class UpdateRolesDto
    {
        public required string UserId { get; set; }
        public List<string>? Roles { get; set; }
    }
}

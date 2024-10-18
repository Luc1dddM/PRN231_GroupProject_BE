using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Dtos
{
    public class UpdateRoleDto
    {
        public string RoleId { get; set; }
        public required string name { get; set; }
        public string[]? permissions { get; set; }
    }
}

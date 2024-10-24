using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Dtos
{
    public class PermissionManagerDto
    {
        public string Type { get; set; }
        public string[] Permissions { get; set; }
    }
}

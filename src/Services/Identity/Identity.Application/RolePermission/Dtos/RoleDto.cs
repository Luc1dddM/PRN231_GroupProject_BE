using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.RolePermission.Dtos
{
    public class RoleDto
    {
        public string RoleId { get; set; }
        public string Name { get; set; }
        public string[] Permissions { get; set; }
    }
}

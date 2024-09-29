using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class RolePermission
    {
        public required string RoleId { get; set; }
        public int PermissionId { get; set; }   
    }
}

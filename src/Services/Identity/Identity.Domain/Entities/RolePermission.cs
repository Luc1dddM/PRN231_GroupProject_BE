using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class RolePermission
    {
        public required string RoleId { get; set; }
        [ForeignKey("RoleId")]
        public int PermissionId { get; set; }   
        public Role Role { get; set; }
        public Permission Permission { get; set; }

    }
}

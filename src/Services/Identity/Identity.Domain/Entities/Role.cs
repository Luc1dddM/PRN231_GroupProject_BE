using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class Role : IdentityRole<int>
    {
        public string RoleId { get; set; } = Guid.NewGuid().ToString();
        public virtual ICollection<Permission>? Permissions { get; set; } 
    }
}

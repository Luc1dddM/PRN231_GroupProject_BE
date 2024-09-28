using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class Role : IdentityRole
    {
        public virtual ICollection<Permission>? Permissions { get; set; } 
        public DateTime? CreatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }
        public string? UpdatedAt { get; private set; }
    }
}

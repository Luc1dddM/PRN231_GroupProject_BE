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
        public DateTime? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedAt { get; set; }
    }
}

using Microsoft.AspNetCore.Authorization;

namespace Identity.Infrastructure.Identity.Configuration
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission {  get; set; } 
        public PermissionRequirement(string permission)
        {
            Permission = permission;    
        }
    }
}

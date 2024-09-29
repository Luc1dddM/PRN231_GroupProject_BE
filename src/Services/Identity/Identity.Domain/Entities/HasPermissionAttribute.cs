using Microsoft.AspNetCore.Authorization;

namespace Identity.Domain.Entities
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(Domain.Enums.Permission permission) : base(policy: permission.ToString()) {
        
        }
    }
}

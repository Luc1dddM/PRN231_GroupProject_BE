using Identity.Infrastructure.Identity.Configuration;
using Identity.Infrastructure.Identity.Utils;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace Identity.Infrastructure.Identity.Handler
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var permissionss = context
                .User.Claims.Where(x => x.Type == CustomClaims.Permissions)
                .Select(x => x.Value)
                .ToHashSet();

            if (permissionss.Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}

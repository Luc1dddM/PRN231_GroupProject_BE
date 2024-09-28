using Identity.Application.Identity.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Identity.Services
{
    public class PermissionSerivce : IPermissionService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public PermissionSerivce(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<List<string>> GetPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User Not Found!");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var rolesWithPermissions = await _context.Roles
                .Where(r => roles.Contains(r.Name))
                .Include(r => r.Permissions)
                .SelectMany(r => r.Permissions)
                .Select(r => r.Name)
                .ToListAsync();

            return rolesWithPermissions;
        }
    }
}

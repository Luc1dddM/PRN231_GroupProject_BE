using BuildingBlocks.Models;
using Identity.Application.RolePermission.Interfaces;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CUser = Identity.Domain.Entities.User;

namespace Identity.Infrastructure.RolePermission.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly UserManager<CUser> _userManager;
        private readonly ApplicationDbContext _context;
        public RolePermissionService(UserManager<CUser> userManager, ApplicationDbContext context)
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

        public async Task<BaseResponse<bool>> UpdatePermission(string Role, List<int>? PermissionIds)
        {


            var role = await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id.Equals(Role));


            if (role is null) throw new Exception("Role Not Found!");


            role.Permissions?.Clear();

            await _context.SaveChangesAsync();

            if (PermissionIds?.Count > 0)
            {
                foreach (var permissionId in PermissionIds)
                {
                    var permission = GetPermissionById(permissionId);
                    if (permission != null)
                        role?.Permissions?.Add(permission);
                }
            }

            await _context.SaveChangesAsync();
            return new BaseResponse<bool>(true);
        }

        public Task<BaseResponse<bool>> AddNewRole(string name, string userId)
        {
            if (GetUserById(userId) is null) throw new Exception("User not found");

            Role newRole = new Role()
            {
                Name = name,
                NormalizedName = name.ToUpper(),
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
            };

            _context.Roles.Add(newRole);
            _context.SaveChangesAsync();
            return Task.FromResult(new BaseResponse<bool>(true));
        }

        public async Task<BaseResponse<bool>> UpdateRoles(string UserId, List<string>? Roles)
        {
            var user = GetUserById(UserId); if (user == null) throw new Exception("User not found");
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0) await _userManager.RemoveFromRolesAsync(user, roles);

            if (Roles != null && Roles?.Count > 0)
                await _userManager.AddToRolesAsync(user, Roles);
            return new BaseResponse<bool>(true);
        }
        private Permission GetPermissionById(int id) => _context.Permissions.FirstOrDefault(r => r.Id == id);

        private CUser GetUserById(string id) => _context.Users.FirstOrDefault(r => r.Id == id);

        
    }
}

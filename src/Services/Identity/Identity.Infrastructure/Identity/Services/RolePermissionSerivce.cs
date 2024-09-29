using Identity.Application.Dtos;
using Identity.Application.Identity.Interfaces;
using Identity.Application.RolePermission.Interfaces;
using Identity.Application.Utils;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Identity.Services
{
    public class RolePermissionSerivce : IRolePermissionService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public RolePermissionSerivce(UserManager<User> userManager, ApplicationDbContext context)
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

        public async Task<BaseResponse<bool>> UpdatePermission(UpdatePermissionDto updateDto)
        {

           
            var role = await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id.Equals(updateDto.role));


            if (role is null) throw new Exception("Role Not Found!");


            role.Permissions?.Clear();

            await _context.SaveChangesAsync();

            if(updateDto.permissionIds?.Count > 0) {
                foreach (var permissionId in updateDto.permissionIds)
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
        private Permission GetPermissionById(int id) => _context.Permissions.FirstOrDefault(r => r.Id == id);

        private User GetUserById(string id) => _context.Users.FirstOrDefault(r => r.Id == id);

    }
}

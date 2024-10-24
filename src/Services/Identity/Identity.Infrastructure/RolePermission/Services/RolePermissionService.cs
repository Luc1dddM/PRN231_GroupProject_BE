using BuildingBlocks.Exceptions;
using BuildingBlocks.Models;
using Identity.Application.RolePermission.Dtos;
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new NotFoundException("User Not Found!");
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


        public async Task<BaseResponse<List<RoleDto>>> GetListRole()
        {
            var roles = _context.Roles.Where(r => r.Name != "Customer").Include(r => r.Permissions).Select(r => new RoleDto()
            {
                RoleId = r.RoleId,
                Name = r.Name,
                Permissions = r.Permissions.Select(p => p.Name).ToArray()
            }).ToList();

            return new BaseResponse<List<RoleDto>>(roles);
        }


        public async Task<BaseResponse<List<PermissionManagerDto>>> GetListPermissions(){
            var permissions = _context.Permissions.GroupBy(p => p.TypePermission)
                            .Select(p => new PermissionManagerDto()
                            {
                                Type = p.Key,
                                Permissions = p.Select(p => p.Name).ToArray()
                            }).ToList();
            return new BaseResponse<List<PermissionManagerDto>>(permissions);
        }   

        public async Task<BaseResponse<bool>> UpdatePermission(string Role, List<int>? PermissionIds)
        {
            var role = await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id.Equals(Role));

            if (role is null) throw new NotFoundException("Role Not Found!");

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

        public async Task<BaseResponse<RoleDto>> AddNewRole(string name, string[] permissions)
        {

            Role newRole = new Role()
            {
                Name = name,
                NormalizedName = name.ToUpper(),
            };

            var permissionEntities = await _context.Permissions
           .Where(p => permissions.Contains(p.Name))
           .ToListAsync();

            // Create RolePermission relationships
            foreach (var permission in permissionEntities)
            {
                var rolePermission = new Domain.Entities.RolePermission
                {
                    RoleId = newRole.RoleId,   // The generated Role ID
                    PermissionId = permission.Id // The corresponding Permission ID
                };
                _context.RolePermission.Add(rolePermission);
            }
            _context.Roles.Add(newRole);
            await _context.SaveChangesAsync();

            return new BaseResponse<RoleDto>(new RoleDto() { RoleId = newRole.RoleId, Name = newRole.Name, Permissions = permissions });
        }

        public async Task<BaseResponse<RoleDto>> UpdateRole(string id,string name, string[] permissions)
        {
            try
            {
                var role = await _context.Roles.SingleOrDefaultAsync(r => r.RoleId == id);
                if (role == null) throw new NotFoundException("Role Not Found");

                var permissionEntities = await _context.Permissions
               .Where(p => permissions.Contains(p.Name))
               .ToListAsync();

                var existPermissions = _context.RolePermission.Where(rp => rp.RoleId == id);
                _context.RolePermission.RemoveRange(existPermissions);
                await _context.SaveChangesAsync();

                role.Name = name;
                role.NormalizedName = name.ToUpper();
                role.Permissions = permissionEntities;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

            }
            

            return new BaseResponse<RoleDto>(new RoleDto() { RoleId = id, Name = name, Permissions = permissions });
        }

        public async Task<BaseResponse<bool>> UpdateUserRoles(string UserId, List<string>? Roles)
        {
            var user = GetUserById(UserId); if (user == null) throw new NotFoundException("User not found");
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Count > 0) await _userManager.RemoveFromRolesAsync(user, roles);

            if (Roles != null && Roles?.Count > 0)
                await _userManager.AddToRolesAsync(user, Roles);
            return new BaseResponse<bool>(true);
        }
        private Permission GetPermissionById(int id) => _context.Permissions.FirstOrDefault(r => r.Id == id);

        private CUser GetUserById(string id) => _context.Users.FirstOrDefault(r => r.UserId == id);

        
    }
}

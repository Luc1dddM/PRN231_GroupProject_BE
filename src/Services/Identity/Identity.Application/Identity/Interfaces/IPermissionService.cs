using Identity.Domain.Entities;

namespace Identity.Application.Identity.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermissionsAsync(string userId);
    }
}

using Microsoft.EntityFrameworkCore;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Data.Configurations
{
    internal class RolePermissionConfiguration : IEntityTypeConfiguration<Domain.Entities.RolePermission>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.RolePermission> builder)
        {
            builder.HasKey(x => new {x.RoleId,x.PermissionId});
            builder.HasData(Create("2c5e174e-3b0e-446f-86af-483d56fd7210", Domain.Enums.Permission.ReadUser));
        }

        private static Domain.Entities.RolePermission Create(string roleId, Domain.Enums.Permission permission)
        {
            return new Domain.Entities.RolePermission
            {
                RoleId = roleId,
                PermissionId = (int)permission
            };
        }
    }
}

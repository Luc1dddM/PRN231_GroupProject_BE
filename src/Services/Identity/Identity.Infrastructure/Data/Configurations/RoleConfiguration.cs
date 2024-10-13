using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Hosting;
using System.Reflection.Emit;

namespace Identity.Infrastructure.Data.Configurations
{
    internal class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany(x => x.Permissions)
                    .WithMany(x => x.Roles)
                    .UsingEntity<Domain.Entities.RolePermission>(
                        l => l.HasOne(r => r.Permission).WithMany().HasForeignKey("PermissionId").HasPrincipalKey(nameof(Permission.Id)),
                        r => r.HasOne(r => r.Role).WithMany().HasForeignKey("RoleId").HasPrincipalKey(nameof(Role.RoleId)));

            builder.HasData(new Role() {Id = 1, RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "ADMIN".ToUpper() });
        }
    }
}

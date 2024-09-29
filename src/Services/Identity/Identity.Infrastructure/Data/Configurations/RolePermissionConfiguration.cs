using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data.Configurations
{
    internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey(x => new {x.RoleId,x.PermissionId});
            builder.HasData(Create("2c5e174e-3b0e-446f-86af-483d56fd7210", Domain.Enums.Permission.ReadUser));
        }

        private static RolePermission Create(string roleId, Domain.Enums.Permission permission)
        {
            return new RolePermission
            {
                RoleId = roleId,
                PermissionId = (int)permission
            };
        }
    }
}

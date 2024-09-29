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
    internal class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");
            builder.HasKey(x => x.Id);

            IEnumerable<Permission> permissions = Enum.GetValues<Domain.Enums.Permission>()
                .Select(p => new Permission
                {
                    Id = (int)p,
                    Name = p.ToString(),
                });

            builder.HasData(permissions);
        }
    }
}

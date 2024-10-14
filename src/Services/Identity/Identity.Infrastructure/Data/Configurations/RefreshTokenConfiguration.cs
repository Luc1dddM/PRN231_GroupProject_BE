using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder
            .HasKey(rt => rt.Id); // Set the primary key of RefreshToken

            builder
                 .HasOne(rt => rt.User) // Each RefreshToken belongs to one User
                 .WithOne(u => u.RefreshToken) // Each User has one RefreshToken
                 .HasPrincipalKey<Domain.Entities.User>(u => u.UserId) // UserId in User is the principal key
                 .HasForeignKey<RefreshToken>(rt => rt.UserId); // UserId in RefreshToken is the foreign key
        }
    }
}

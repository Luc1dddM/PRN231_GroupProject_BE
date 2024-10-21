using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Data.Configurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<Domain.Entities.User>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.User> builder)
        {
            // Self-referencing relationship for CreatedBy
            builder
                .HasOne(u => u.CreatedByUser)
                .WithMany()
                .HasForeignKey(u => u.CreatedBy)
                .HasPrincipalKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Self-referencing relationship for UpdatedBy
            builder
                .HasOne(u => u.UpdatedByUser)
                .WithMany()
                .HasForeignKey(u => u.UpdatedBy)
                .HasPrincipalKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}

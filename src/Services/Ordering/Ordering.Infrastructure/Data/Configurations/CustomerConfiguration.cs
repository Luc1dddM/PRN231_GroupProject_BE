
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id); //this "Id" here is of type CustomerId, we need to convert it to something that EF can know to store in db 
            builder.Property(c => c.Id).HasConversion(
                    customerId => customerId.Value,
                    dbId => CustomerId.Of(dbId)); //convert the "Id" from CustomerId to Guid(as default of HasConversion)

            builder.Property(c => c.Name).HasMaxLength(100).IsRequired();

            builder.Property(c => c.Email).HasMaxLength(255);

            builder.HasIndex(c => c.Email).IsUnique();
        }
    }
}

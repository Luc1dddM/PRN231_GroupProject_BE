using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            // Primary Key (integer)
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).ValueGeneratedOnAdd();

            // Domain Identifier with unique constraint
            builder.Property(oi => oi.EntityId).HasConversion( //save in db as a guid but read data as a OrderItemId type
                                       orderItemId => orderItemId.Value,
                                       dbId => OrderItemId.Of(dbId));
            builder.HasIndex(oi => oi.EntityId).IsUnique();

            // Foreign Key (using Order's EntityId)
            builder.Property(oi => oi.OrderId).HasConversion(
                                        ordId => ordId.Value,
                                        dbordId => OrderId.Of(dbordId)).IsRequired();

            // Other properties
            builder.Property(oi => oi.ProductId).HasConversion(
                                        proId => proId.Value,
                                        dbproId => ProductId.Of(dbproId)).IsRequired();

            builder.Property(oi => oi.Quantity).IsRequired();

            builder.Property(oi => oi.Price).IsRequired();

            builder.Property(oi => oi.Color).IsRequired();

        }
    }
}

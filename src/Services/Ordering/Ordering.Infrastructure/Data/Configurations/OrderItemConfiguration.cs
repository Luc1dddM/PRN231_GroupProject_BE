using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).HasConversion( //save in db as a guid but read data as a OrderItemId type
                                       orderItemId => orderItemId.Value,
                                       dbId => OrderItemId.Of(dbId));

            builder.Property(oi => oi.ProductId).HasConversion(
                                        proId => proId.Value,
                                        dbproId => ProductId.Of(dbproId)).IsRequired();

            builder.Property(oi => oi.Quantity).IsRequired();

            builder.Property(oi => oi.Price).IsRequired();

            builder.Property(oi => oi.Color).IsRequired();

        }
    }
}

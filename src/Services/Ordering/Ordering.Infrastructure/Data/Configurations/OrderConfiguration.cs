using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Ordering.Domain.Enums;
using Ordering.Domain.Models;
using Ordering.Domain.ValueObjects;

namespace Ordering.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // Primary Key (integer)
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            // Domain Identifier with unique constraint
            builder.Property(o => o.EntityId).HasConversion(
                            orderId => orderId.Value,
                            dbId => OrderId.Of(dbId));
            builder.HasIndex(o => o.EntityId).IsUnique();

            //make EntityId an alternate key to be used in the relationship
            builder.HasAlternateKey(o => o.EntityId);

            // Relationship with OrderItems using EntityId
            builder.HasMany(o => o.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId)
                .HasPrincipalKey(o => o.EntityId);

            //other normal properties
            builder.Property(o => o.CustomerId).HasConversion(
                                        customerId => customerId.Value,
                                        dbcustomerId => CustomerId.Of(dbcustomerId)).IsRequired();

            builder.ComplexProperty(
                o => o.ShippingAddress, addressBuilder =>
                    {
                        addressBuilder.Property(a => a.FirstName)
                            .HasMaxLength(50)
                            .IsRequired();

                        addressBuilder.Property(a => a.LastName)
                             .HasMaxLength(50)
                             .IsRequired();

                        addressBuilder.Property(a => a.Phone)
                            .HasMaxLength(10)
                            .IsRequired();

                        addressBuilder.Property(a => a.EmailAddress)
                            .HasMaxLength(50);

                        addressBuilder.Property(a => a.AddressLine)
                            .HasMaxLength(180)
                            .IsRequired();

                        addressBuilder.Property(a => a.City)
                            .HasMaxLength(50)
                            .IsRequired();

                        addressBuilder.Property(a => a.District)
                            .HasMaxLength(50)
                            .IsRequired();

                        addressBuilder.Property(a => a.Ward)
                            .HasMaxLength(50)
                            .IsRequired();
                    }
            );


            builder.ComplexProperty(
               o => o.Payment, paymentBuilder =>
               {
                   paymentBuilder.Property(p => p.CardName)
                       .HasMaxLength(50);

                   paymentBuilder.Property(p => p.CardNumber)
                       .HasMaxLength(24)
                       .IsRequired();

                   paymentBuilder.Property(p => p.Expiration)
                       .HasMaxLength(10);

                   paymentBuilder.Property(p => p.CVV)
                       .HasMaxLength(3);

                   paymentBuilder.Property(p => p.PaymentMethod);
               }
            );

            builder.Property(o => o.Status)
            .HasDefaultValue(OrderStatus.Pending)
            .HasConversion(
                s => s.ToString(),
                dbStatus => (OrderStatus)Enum.Parse(typeof(OrderStatus), dbStatus));

            builder.Property(o => o.CouponId);

            builder.Property(o => o.TotalPrice);
            //-----------------------------------------------------
        }
    }
}

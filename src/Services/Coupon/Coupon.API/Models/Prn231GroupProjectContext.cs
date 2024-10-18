using Microsoft.EntityFrameworkCore;

namespace Coupon.API.Models
{
    public class Prn231GroupProjectContext : DbContext
    {
        public Prn231GroupProjectContext(DbContextOptions<Prn231GroupProjectContext> options)
           : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }
        public async Task UpdateCouponStatusIfNeeded(string couponCode)
        {
            var coupon = await Coupons.FirstOrDefaultAsync(c => c.CouponCode == couponCode);

            if (coupon != null && coupon.Quantity <= 0)
            {
                coupon.Status = false;
                await SaveChangesAsync();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Coupon>().HasData(
                new Coupon
                {
                    Id = 1,
                    CouponId = Guid.NewGuid().ToString(),
                    CouponCode = "WELCOME20",
                    DiscountAmount = 20.0,
                    Quantity = 10,
                    Status = true,
                    MinAmount = 50,
                    MaxAmount = 200,
                    CreatedBy = "Admin",
                    CreatedDate = DateTime.UtcNow,
                },
                new Coupon
                {
                    Id = 2,
                    CouponId = Guid.NewGuid().ToString(),
                    CouponCode = "SUMMER2024",
                    DiscountAmount = 50.0,
                    Quantity = 5,
                    Status = true,
                    MinAmount = 100,
                    MaxAmount = 500,
                    CreatedBy = "Admin",
                    CreatedDate = DateTime.UtcNow,
                }
            );

        }
    } 
}

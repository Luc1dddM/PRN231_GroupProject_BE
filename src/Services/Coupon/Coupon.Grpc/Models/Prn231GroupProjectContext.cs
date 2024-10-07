using Microsoft.EntityFrameworkCore;

namespace Coupon.Grpc.Models
{
    public class Prn231GroupProjectContext : DbContext
    {
        public Prn231GroupProjectContext(DbContextOptions<Prn231GroupProjectContext> options)
           : base(options)
        {
        }

        public DbSet<Coupon> Coupons { get; set; }

    }
}

using Microsoft.EntityFrameworkCore;
using ShoppingCart.API.Models;

namespace WebApplication1.Models
{
    public class CartDBContext : DbContext
    {
        public CartDBContext(DbContextOptions<CartDBContext> options) : base(options)
        {
        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }

    }
}

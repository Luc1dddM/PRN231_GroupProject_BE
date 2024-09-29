namespace ShoppingCart.API.Models
{
    public class CartDBContext : DbContext
    {
        public CartDBContext(DbContextOptions<CartDBContext> options) : base(options)
        {
        }

        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartHeader>().HasIndex(c => c.CartHeaderId).IsUnique();

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.CartHeader)
                .WithMany(cd => cd.CartDetails)
                .HasForeignKey(cd => cd.CartHeaderId)
                .HasPrincipalKey(ch => ch.CartHeaderId);
        }
    }
}

using Microsoft.EntityFrameworkCore;

namespace Feedback.API.Models;
public class Prn231GroupProjectContext : DbContext
{
    public Prn231GroupProjectContext(DbContextOptions<Prn231GroupProjectContext> options)
        : base(options)
    {
    }

    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id); 
            entity.Property(e => e.FeedbackId)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .IsRequired();

            entity.Property(e => e.ProductId)
                  .IsRequired();

            entity.Property(e => e.OrderId)
                  .IsRequired();

            entity.Property(e => e.Rating)
                  .IsRequired();

            entity.Property(e => e.RateBy)
                .IsRequired();

            entity.Property(e => e.DatePost)
                  .IsRequired();
        });

    }
}

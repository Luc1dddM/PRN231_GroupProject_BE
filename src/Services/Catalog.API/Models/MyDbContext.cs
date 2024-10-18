using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
                {
                    entity.Property(e => e.ProductId)
                        .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");
                    entity.HasIndex(e => e.ProductId).IsUnique();

                    entity.HasData(
                        new Product
                        {
                            Id = 1,
                            ProductId = Guid.NewGuid().ToString(),
                            Name = "Razer Pro Click Humanscale Mouse | Wireless",
                            Price = 2290000,
                            Description = "Razer Pro Click Humanscale Mouse | Wireless",
                            ImageUrl = "Test.jpg",
                            CreateBy = "Test",
                            CreateDate = DateTime.Now,
                            UpdateBy = "Test",
                            UpdateDate = DateTime.Now,
                            Status = true
                        },
                        new Product
                        {
                            Id = 2,
                            ProductId = Guid.NewGuid().ToString(),
                            Name = "Razer DeathAdder V2 Pro Mouse | Wireless",
                            Price = 1990000,
                            Description = "Razer DeathAdder V2 Pro Mouse | Wireless",
                            ImageUrl = "Test.jpg",
                            CreateBy = "Test",
                            CreateDate = DateTime.Now,
                            UpdateBy = "Test",
                            UpdateDate = DateTime.Now,
                            Status = true
                        }
                        );
                }
            );

            modelBuilder.Entity<Category>(entity =>
                {
                    entity.Property(e => e.CategoryId)
                        .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");
                    entity.HasIndex(e => e.CategoryId).IsUnique();
                    entity.HasData(
                        new Category
                        {
                            Id = 1,
                            CategoryId = Guid.NewGuid().ToString(),
                            Name = "Asus",
                            Type = "Brand",
                            CreatedBy = "Test",
                            CreatedAt = DateTime.Now,
                            UpdatedBy = "Test",
                            UpdatedAt = DateTime.Now,
                            Status = true
                        },
                        new Category
                        {
                            Id = 2,
                            CategoryId = Guid.NewGuid().ToString(),
                            Name = "Razer",
                            Type = "Brand",
                            CreatedBy = "Test",
                            CreatedAt = DateTime.Now,
                            UpdatedBy = "Test",
                            UpdatedAt = DateTime.Now,
                            Status = true
                        }
                        );
                }
            );

            modelBuilder.Entity<ProductCategory>(entity =>
                {
                    entity.Property(e => e.ProductCategoryId)
                         .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");
                    entity.HasIndex(e => e.ProductCategoryId).IsUnique();
                    entity.HasOne(d => d.Product).WithMany(p => p.ProductCategories)
                         .HasPrincipalKey(p => p.ProductId)
                        .HasForeignKey(d => d.ProductId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Product_Category_Product");
                    entity.HasOne(d => d.Category).WithMany(p => p.ProductCategories)
                        .HasPrincipalKey(p => p.CategoryId)
                        .HasForeignKey(d => d.CategoryId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Product_Category_Category");
                }
             );

        }
    }
}

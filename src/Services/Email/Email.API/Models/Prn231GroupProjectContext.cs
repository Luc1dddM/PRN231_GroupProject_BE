using Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Models
{
    public class Prn231GroupProjectContext : DbContext
    {

        public Prn231GroupProjectContext(DbContextOptions<Prn231GroupProjectContext> options)
            : base(options)
        {
        }

        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<EmailSend> EmailSends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailSend>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Receiver)
                      .IsRequired();

            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Subject).IsRequired();
                entity.Property(e => e.Body).IsRequired();
            });
        }
    }

    }



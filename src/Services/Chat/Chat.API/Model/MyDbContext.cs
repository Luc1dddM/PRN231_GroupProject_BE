using Microsoft.EntityFrameworkCore;

namespace Chat.API.Model
{
    public class MyDbContext: DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options):base(options)
        {
            
        }

        public virtual DbSet<ConnectionUser> ConnectionUsers { get; set; }
        public virtual DbSet<Group> Groups { get; set; }    
        public virtual DbSet<GroupMember> GroupMembers { get; set; }
        public virtual DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ConnectionUser>(entity =>
            {
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.Property(e => e.GroupId)
                     .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");
                entity.HasIndex(e => e.GroupId).IsUnique();
            });

            modelBuilder.Entity<GroupMember>(entity =>
            {
                entity.Property(e => e.GroupMemberId)
                     .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");
                entity.HasIndex(e => e.GroupMemberId).IsUnique();
                entity.HasOne(d => d.Group).WithMany(p => p.GroupMembers)
                     .HasPrincipalKey(p => p.GroupId)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Group_Member_Group");
                entity.HasOne(d => d.User).WithMany(p => p.GroupMembers)
                    .HasPrincipalKey(p => p.UserId)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Group_Member_ConnectionUser");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(e => e.MessageId)
                     .HasDefaultValueSql("(CONVERT([nvarchar](36),newid()))");
                entity.HasIndex(e => e.MessageId).IsUnique();
                entity.HasOne(d => d.Group).WithMany(p => p.Messages)
                     .HasPrincipalKey(p => p.GroupId)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_Group");
                entity.HasOne(d => d.User).WithMany(p => p.Messages)
                    .HasPrincipalKey(p => p.UserId)
                    .HasForeignKey(d => d.SenderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Message_ConnectionUser");
            }
 );
        }
    }
}

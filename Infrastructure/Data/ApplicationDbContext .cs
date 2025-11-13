using Core.Entities;
using Core.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(r => r.Name).IsUnique();

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                // Seed data
                entity.HasData(
                    new Role { Id = 1, Name = "Employee" },
                    new Role { Id = 2, Name = "Manager" }
                );
            });

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();

                entity.Property(u => u.Username)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // LeaveRequest
            modelBuilder.Entity<LeaveRequest>(entity =>
            {
                entity.HasIndex(lr => new { lr.UserId, lr.LeaveStatusId });

                entity.Property(lr => lr.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(lr => lr.Reason)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(lr => lr.RejectionReason)
                    .HasMaxLength(500);

                entity.Property(lr => lr.RowVersion)
                        .IsRowVersion()
                        .IsConcurrencyToken();

                entity.HasOne(lr => lr.User)
                    .WithMany(u => u.LeaveRequests)
                    .HasForeignKey(lr => lr.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.CreatedUser)
                    .WithMany()
                    .HasForeignKey(lr => lr.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.UpdatedUser)
                    .WithMany() 
                    .HasForeignKey(lr => lr.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Approval
            modelBuilder.Entity<Approval>(entity =>
            {
                entity.Property(a => a.Comments)
                    .HasMaxLength(1000);

                modelBuilder.Entity<Approval>()
                    .Property(a => a.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(a => a.LeaveRequest)
                    .WithMany(lr => lr.Approvals)
                    .HasForeignKey(a => a.LeaveRequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Manager)
                    .WithMany(u => u.Approvals)
                    .HasForeignKey(a => a.ManagerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.CreatedUser)
                    .WithMany()
                    .HasForeignKey(lr => lr.CreatedBy)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(lr => lr.UpdatedUser)
                    .WithMany()
                    .HasForeignKey(lr => lr.UpdatedBy)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }
}
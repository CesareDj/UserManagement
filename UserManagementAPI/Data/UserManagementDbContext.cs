using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UserManagementAPI.Models;

namespace UserManagementAPI.Data
{
    public class UserManagementDbContext : DbContext
    {
        public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
        {
        }

        public UserManagementDbContext()
        {
        }

        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=UserManagement.db");
            }
        }

        public override int SaveChanges()
        {
            DateTime currentTime = DateTime.Now;

            foreach (EntityEntry entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
            {
                if (entry.Entity is BaseEntity entity)
                {
                    entity.CreatedAt = currentTime;
                    entity.CreatedBy = "System";
                }
            }

            foreach (EntityEntry entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
            {
                if (entry.Entity is BaseEntity entity)
                {
                    entity.UpdatedAt = currentTime;
                    entity.UpdatedBy = "System";
                }
            }

            foreach (EntityEntry entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Metadata.GetProperties().Any(x => x.Name == "DeletedAt")))
            {
                entry.State = EntityState.Unchanged;
                entry.CurrentValues["DeletedAt"] = currentTime;
                entry.CurrentValues["DeletedBy"] = "System";
            }

            return base.SaveChanges();
        }
    }
}
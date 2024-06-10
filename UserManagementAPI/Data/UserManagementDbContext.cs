using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UserManagementAPI.Models;

public class UserManagementDbContext : DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Country> Countries { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<User> Users { get; set; }

    public override int SaveChanges()
    {
        var currentTime = DateTime.Now;

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
        {
            if (entry.Entity is BaseEntity entity)
            {
                entity.CreatedAt = currentTime;
            }
        }

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is BaseEntity entity)
            {
                entity.UpdatedAt = currentTime;
            }
        }

        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted && e.Metadata.GetProperties().Any(x => x.Name == "DeletedAt")))
        {
            entry.State = EntityState.Unchanged;
            entry.CurrentValues["DeletedAt"] = currentTime;
        }

        return base.SaveChanges();
    }
}
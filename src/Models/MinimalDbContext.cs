using Microsoft.EntityFrameworkCore;

namespace api.Models;

public class MinimalDbContext : DbContext
{
    public MinimalDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "name 1" },
            new User { Id = 2, Name = "name 2" },
            new User { Id = 3, Name = "name 3" },
            new User { Id = 4, Name = "name 4" },
            new User { Id = 5, Name = "name 5" }
        );
    }
}

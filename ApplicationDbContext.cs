using Microsoft.EntityFrameworkCore;

namespace botilleria_clean_architecture_api;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Add DbSets here as needed, e.g.:
    // public DbSet<Product> Products { get; set; }
}
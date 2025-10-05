using Microsoft.EntityFrameworkCore;

namespace botilleria_clean_architecture_api;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets for products and related entities
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<ProductType> ProductTypes { get; set; } = null!;
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Country> Countries { get; set; } = null!;
    public DbSet<Region> Regions { get; set; } = null!;
    public DbSet<Origin> Origins { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(b =>
        {
            b.Property(p => p.Price).HasPrecision(18, 2);
            b.Property(p => p.DiscountPrice).HasPrecision(18, 2);
            b.OwnsOne(p => p.Characteristics, cb =>
            {
                cb.Property(c => c.Color);
                cb.Property(c => c.Aroma);
                cb.Property(c => c.Taste);
                cb.Property(c => c.ServingTemperature);
                cb.Property(c => c.FoodPairingJson).HasColumnName("FoodPairing");
            });
        });

        modelBuilder.Entity<Country>().HasIndex(c => c.IsoCode);
        modelBuilder.Entity<Category>().HasIndex(c => c.Name);
        modelBuilder.Entity<Brand>().HasIndex(b => b.Name);
        modelBuilder.Entity<ProductType>().HasIndex(t => t.Name);
    }
}
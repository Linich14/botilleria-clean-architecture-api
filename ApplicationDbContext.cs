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

        // Product configuration
        modelBuilder.Entity<Product>(b =>
        {
            b.Property(p => p.Price).HasPrecision(18, 2);
            b.Property(p => p.DiscountPrice).HasPrecision(18, 2);
            b.Property(p => p.AlcoholContent).HasPrecision(5, 2);
            
            b.OwnsOne(p => p.Characteristics, cb =>
            {
                cb.Property(c => c.Color).HasMaxLength(50);
                cb.Property(c => c.Aroma).HasMaxLength(500);
                cb.Property(c => c.Taste).HasMaxLength(500);
                cb.Property(c => c.ServingTemperature).HasMaxLength(50);
                cb.Property(c => c.FoodPairingJson).HasColumnName("FoodPairing");
            });

            // Relationships
            b.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            b.HasOne(p => p.ProductType)
                .WithMany(t => t.Products)
                .HasForeignKey(p => p.ProductTypeId);

            b.HasOne(p => p.Brand)
                .WithMany(br => br.Products)
                .HasForeignKey(p => p.BrandId);

            b.HasOne(p => p.Origin)
                .WithMany(o => o.Products)
                .HasForeignKey(p => p.OriginId);
        });

        // Region configuration
        modelBuilder.Entity<Region>(b =>
        {
            b.HasOne(r => r.Country)
                .WithMany(c => c.Regions)
                .HasForeignKey(r => r.CountryId);
        });

        // Origin configuration
        modelBuilder.Entity<Origin>(b =>
        {
            b.HasOne(o => o.Country)
                .WithMany(c => c.Origins)
                .HasForeignKey(o => o.CountryId);

            b.HasOne(o => o.Region)
                .WithMany(r => r.Origins)
                .HasForeignKey(o => o.RegionId);
        });

        // Indexes
        modelBuilder.Entity<Country>().HasIndex(c => c.IsoCode).IsUnique();
        modelBuilder.Entity<Category>().HasIndex(c => c.Name);
        modelBuilder.Entity<Brand>().HasIndex(b => b.Name);
        modelBuilder.Entity<ProductType>().HasIndex(t => t.Name);
        modelBuilder.Entity<Product>().HasIndex(p => p.Name);
    }
}
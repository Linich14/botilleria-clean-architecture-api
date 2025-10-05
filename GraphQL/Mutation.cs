using HotChocolate;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace botilleria_clean_architecture_api.GraphQL;

public class Mutation
{
    public async Task<Product> CreateProduct(DTOs.CreateProductDto input, [Service] ApplicationDbContext db)
    {
        // Simplified: create or find related entities by name/id
        Category? category = null;
        if (input.Category?.Id is > 0) category = await db.Categories.FindAsync(input.Category.Id);
        else if (!string.IsNullOrWhiteSpace(input.Category?.Name))
        {
            category = await db.Categories.FirstOrDefaultAsync(c => c.Name == input.Category.Name);
            if (category == null) { category = new Category { Name = input.Category.Name, Subcategory = input.Category.Subcategory }; db.Categories.Add(category); }
        }

        Brand? brand = null;
        if (input.Brand?.Id is > 0) brand = await db.Brands.FindAsync(input.Brand.Id);
        else if (!string.IsNullOrWhiteSpace(input.Brand?.Name))
        {
            brand = await db.Brands.FirstOrDefaultAsync(b => b.Name == input.Brand.Name);
            if (brand == null) { brand = new Brand { Name = input.Brand.Name }; db.Brands.Add(brand); }
        }

        ProductType? ptype = null;
        if (input.Type?.Id is > 0) ptype = await db.ProductTypes.FindAsync(input.Type.Id);
        else if (!string.IsNullOrWhiteSpace(input.Type?.Name))
        {
            ptype = await db.ProductTypes.FirstOrDefaultAsync(t => t.Name == input.Type.Name);
            if (ptype == null) { ptype = new ProductType { Name = input.Type.Name }; db.ProductTypes.Add(ptype); }
        }

        Country? country = null;
        if (input.Origin?.Country?.Id is > 0) country = await db.Countries.FindAsync(input.Origin.Country.Id);
        else if (!string.IsNullOrWhiteSpace(input.Origin?.Country?.Name))
        {
            country = await db.Countries.FirstOrDefaultAsync(c => c.Name == input.Origin.Country.Name);
            if (country == null) { country = new Country { Name = input.Origin.Country.Name, IsoCode = input.Origin.Country.Subcategory }; db.Countries.Add(country); }
        }

        Region? region = null;
        if (input.Origin?.Region?.Id is > 0) region = await db.Regions.FindAsync(input.Origin.Region.Id);
        else if (!string.IsNullOrWhiteSpace(input.Origin?.Region?.Name))
        {
            region = await db.Regions.FirstOrDefaultAsync(r => r.Name == input.Origin.Region.Name);
            if (region == null) { region = new Region { Name = input.Origin.Region.Name }; db.Regions.Add(region); }
        }

        Origin origin = new Origin { Country = country, Region = region, Vineyard = input.Origin?.Vineyard };
        db.Origins.Add(origin);

        var product = new Product
        {
            Name = input.Name,
            Description = input.Description,
            Price = input.Price,
            DiscountPrice = input.DiscountPrice,
            Volume = input.Volume,
            Unit = input.Unit,
            AlcoholContent = input.AlcoholContent,
            Stock = input.Stock,
            IsAvailable = input.IsAvailable,
            CreatedAt = input.CreatedAt ?? DateTime.UtcNow,
            UpdatedAt = input.UpdatedAt,
            Category = category,
            Brand = brand,
            ProductType = ptype,
            Origin = origin,
            Characteristics = input.Characteristics == null ? null : new Characteristics { Color = input.Characteristics.Color, Aroma = input.Characteristics.Aroma, Taste = input.Characteristics.Taste, ServingTemperature = input.Characteristics.ServingTemperature, FoodPairingJson = input.Characteristics.FoodPairing == null ? null : JsonSerializer.Serialize(input.Characteristics.FoodPairing) },
            ReviewsCount = input.Reviews?.Count ?? 0,
            AverageRating = input.Reviews?.AverageRating ?? 0m,
            SelfLink = input.Self?.Link
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return product;
    }
}

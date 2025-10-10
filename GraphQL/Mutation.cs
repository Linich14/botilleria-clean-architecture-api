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

        // Handle Origin with Country and Region (both required by Origin model)
        Origin? origin = null;
        if (input.Origin != null)
        {
            Country? country = null;
            if (input.Origin.Country?.Id is > 0) 
            {
                country = await db.Countries.FindAsync(input.Origin.Country.Id);
            }
            else if (!string.IsNullOrWhiteSpace(input.Origin.Country?.Name))
            {
                country = await db.Countries.FirstOrDefaultAsync(c => c.Name == input.Origin.Country.Name);
                if (country == null) 
                { 
                    // IsoCode is required, use Subcategory or default to first 2 chars of name
                    string isoCode = !string.IsNullOrWhiteSpace(input.Origin.Country.Subcategory) 
                        ? input.Origin.Country.Subcategory 
                        : input.Origin.Country.Name.Length >= 2 
                            ? input.Origin.Country.Name.Substring(0, 2).ToUpper() 
                            : "XX";
                    
                    country = new Country { Name = input.Origin.Country.Name, IsoCode = isoCode }; 
                    db.Countries.Add(country); 
                    await db.SaveChangesAsync(); // Save to get Country ID for FK
                }
            }

            Region? region = null;
            if (input.Origin.Region?.Id is > 0) 
            {
                region = await db.Regions.FindAsync(input.Origin.Region.Id);
            }
            else if (!string.IsNullOrWhiteSpace(input.Origin.Region?.Name) && country != null)
            {
                region = await db.Regions.FirstOrDefaultAsync(r => r.Name == input.Origin.Region.Name && r.CountryId == country.Id);
                if (region == null) 
                { 
                    region = new Region { Name = input.Origin.Region.Name, Country = country }; 
                    db.Regions.Add(region); 
                    await db.SaveChangesAsync(); // Save to get Region ID for FK
                }
            }

            // Only create Origin if both Country and Region exist (both are required by the model)
            if (country != null && region != null)
            {
                origin = new Origin { Country = country, Region = region, Vineyard = input.Origin.Vineyard };
                db.Origins.Add(origin);
            }
        }

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

    public async Task<Product?> UpdateProduct(int id, DTOs.CreateProductDto input, [Service] ApplicationDbContext db)
    {
        // Find existing product
        var product = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductType)
            .Include(p => p.Origin)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null) return null;

        // Update basic fields
        product.Name = input.Name;
        product.Description = input.Description;
        product.Price = input.Price;
        product.DiscountPrice = input.DiscountPrice;
        product.Volume = input.Volume;
        product.Unit = input.Unit;
        product.AlcoholContent = input.AlcoholContent;
        product.Stock = input.Stock;
        product.IsAvailable = input.IsAvailable;
        product.UpdatedAt = DateTime.UtcNow;

        // Update Category
        if (input.Category?.Id is > 0)
        {
            product.Category = await db.Categories.FindAsync(input.Category.Id);
        }
        else if (!string.IsNullOrWhiteSpace(input.Category?.Name))
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == input.Category.Name);
            if (category == null)
            {
                category = new Category { Name = input.Category.Name, Subcategory = input.Category.Subcategory };
                db.Categories.Add(category);
            }
            product.Category = category;
        }

        // Update Brand
        if (input.Brand?.Id is > 0)
        {
            product.Brand = await db.Brands.FindAsync(input.Brand.Id);
        }
        else if (!string.IsNullOrWhiteSpace(input.Brand?.Name))
        {
            var brand = await db.Brands.FirstOrDefaultAsync(b => b.Name == input.Brand.Name);
            if (brand == null)
            {
                brand = new Brand { Name = input.Brand.Name };
                db.Brands.Add(brand);
            }
            product.Brand = brand;
        }

        // Update ProductType
        if (input.Type?.Id is > 0)
        {
            product.ProductType = await db.ProductTypes.FindAsync(input.Type.Id);
        }
        else if (!string.IsNullOrWhiteSpace(input.Type?.Name))
        {
            var ptype = await db.ProductTypes.FirstOrDefaultAsync(t => t.Name == input.Type.Name);
            if (ptype == null)
            {
                ptype = new ProductType { Name = input.Type.Name };
                db.ProductTypes.Add(ptype);
            }
            product.ProductType = ptype;
        }

        // Update Origin
        if (input.Origin != null)
        {
            Country? country = null;
            if (input.Origin.Country?.Id is > 0)
            {
                country = await db.Countries.FindAsync(input.Origin.Country.Id);
            }
            else if (!string.IsNullOrWhiteSpace(input.Origin.Country?.Name))
            {
                country = await db.Countries.FirstOrDefaultAsync(c => c.Name == input.Origin.Country.Name);
                if (country == null)
                {
                    string isoCode = !string.IsNullOrWhiteSpace(input.Origin.Country.Subcategory)
                        ? input.Origin.Country.Subcategory
                        : input.Origin.Country.Name.Length >= 2
                            ? input.Origin.Country.Name.Substring(0, 2).ToUpper()
                            : "XX";

                    country = new Country { Name = input.Origin.Country.Name, IsoCode = isoCode };
                    db.Countries.Add(country);
                    await db.SaveChangesAsync();
                }
            }

            Region? region = null;
            if (input.Origin.Region?.Id is > 0)
            {
                region = await db.Regions.FindAsync(input.Origin.Region.Id);
            }
            else if (!string.IsNullOrWhiteSpace(input.Origin.Region?.Name) && country != null)
            {
                region = await db.Regions.FirstOrDefaultAsync(r => r.Name == input.Origin.Region.Name && r.CountryId == country.Id);
                if (region == null)
                {
                    region = new Region { Name = input.Origin.Region.Name, Country = country };
                    db.Regions.Add(region);
                    await db.SaveChangesAsync();
                }
            }

            if (country != null && region != null)
            {
                // Remove old origin if exists
                if (product.Origin != null)
                {
                    db.Origins.Remove(product.Origin);
                }
                
                var origin = new Origin { Country = country, Region = region, Vineyard = input.Origin.Vineyard };
                db.Origins.Add(origin);
                product.Origin = origin;
            }
        }

        // Update Characteristics
        if (input.Characteristics != null)
        {
            product.Characteristics = new Characteristics
            {
                Color = input.Characteristics.Color,
                Aroma = input.Characteristics.Aroma,
                Taste = input.Characteristics.Taste,
                ServingTemperature = input.Characteristics.ServingTemperature,
                FoodPairingJson = input.Characteristics.FoodPairing == null ? null : JsonSerializer.Serialize(input.Characteristics.FoodPairing)
            };
        }

        await db.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteProduct(int id, [Service] ApplicationDbContext db)
    {
        var product = await db.Products.FindAsync(id);
        if (product == null) return false;

        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return true;
    }
}

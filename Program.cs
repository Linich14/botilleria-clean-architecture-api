using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using botilleria_clean_architecture_api;
using HotChocolate;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Dtos;
using HotChocolate.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Enable Newtonsoft for JSON Patch support
builder.Services.AddControllers().AddNewtonsoftJson();

// JWT Authentication
var key = Encoding.UTF8.GetBytes("secreto123");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };
    });

// GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<botilleria_clean_architecture_api.GraphQL.Query>()
    .AddMutationType<botilleria_clean_architecture_api.GraphQL.Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Products endpoints
app.MapGet("/api/products", async (ApplicationDbContext db) =>
{
    var products = await db.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.ProductType)
        .Include(p => p.Origin!).ThenInclude(o => o.Country)
        .Include(p => p.Origin!).ThenInclude(o => o.Region)
        .ToListAsync();
    return Results.Ok(products);
}).WithName("GetProducts");

app.MapGet("/api/products/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var product = await db.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.ProductType)
        .Include(p => p.Origin!).ThenInclude(o => o.Country)
        .Include(p => p.Origin!).ThenInclude(o => o.Region)
        .FirstOrDefaultAsync(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
}).WithName("GetProductById");

app.MapPost("/api/products", [Microsoft.AspNetCore.Authorization.Authorize] async (CreateProductDto dto, ApplicationDbContext db) =>
{
    // minimal upsert logic similar to GraphQL mutation
    Category? category = null;
    if (dto.Category?.Id is > 0) category = await db.Categories.FindAsync(dto.Category.Id);
    else if (!string.IsNullOrWhiteSpace(dto.Category?.Name))
    {
        category = await db.Categories.FirstOrDefaultAsync(c => c.Name == dto.Category.Name);
        if (category == null) { category = new Category { Name = dto.Category.Name, Subcategory = dto.Category.Subcategory }; db.Categories.Add(category); }
    }

    Brand? brand = null;
    if (dto.Brand?.Id is > 0) brand = await db.Brands.FindAsync(dto.Brand.Id);
    else if (!string.IsNullOrWhiteSpace(dto.Brand?.Name))
    {
        brand = await db.Brands.FirstOrDefaultAsync(b => b.Name == dto.Brand.Name);
        if (brand == null) { brand = new Brand { Name = dto.Brand.Name }; db.Brands.Add(brand); }
    }

    ProductType? ptype = null;
    if (dto.Type?.Id is > 0) ptype = await db.ProductTypes.FindAsync(dto.Type.Id);
    else if (!string.IsNullOrWhiteSpace(dto.Type?.Name))
    {
        ptype = await db.ProductTypes.FirstOrDefaultAsync(t => t.Name == dto.Type.Name);
        if (ptype == null) { ptype = new ProductType { Name = dto.Type.Name }; db.ProductTypes.Add(ptype); }
    }

    Country? country = null;
    if (dto.Origin?.Country?.Id is > 0) country = await db.Countries.FindAsync(dto.Origin.Country.Id);
    else if (!string.IsNullOrWhiteSpace(dto.Origin?.Country?.Name))
    {
        country = await db.Countries.FirstOrDefaultAsync(c => c.Name == dto.Origin.Country.Name);
        if (country == null) { country = new Country { Name = dto.Origin.Country.Name, IsoCode = dto.Origin.Country.Subcategory }; db.Countries.Add(country); }
    }

    Region? region = null;
    if (dto.Origin?.Region?.Id is > 0) region = await db.Regions.FindAsync(dto.Origin.Region.Id);
    else if (!string.IsNullOrWhiteSpace(dto.Origin?.Region?.Name))
    {
        region = await db.Regions.FirstOrDefaultAsync(r => r.Name == dto.Origin.Region.Name);
        if (region == null) { region = new Region { Name = dto.Origin.Region.Name }; db.Regions.Add(region); }
    }

    Origin origin = new Origin { Country = country, Region = region, Vineyard = dto.Origin?.Vineyard };
    db.Origins.Add(origin);

    var product = new Product
    {
        Name = dto.Name,
        Description = dto.Description,
        Price = dto.Price,
        DiscountPrice = dto.DiscountPrice,
        Volume = dto.Volume,
        Unit = dto.Unit,
        AlcoholContent = dto.AlcoholContent,
        Stock = dto.Stock,
        IsAvailable = dto.IsAvailable,
        CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
        UpdatedAt = dto.UpdatedAt,
        Category = category,
        Brand = brand,
        ProductType = ptype,
        Origin = origin,
        Characteristics = dto.Characteristics == null ? null : new Characteristics { Color = dto.Characteristics.Color, Aroma = dto.Characteristics.Aroma, Taste = dto.Characteristics.Taste, ServingTemperature = dto.Characteristics.ServingTemperature, FoodPairingJson = dto.Characteristics.FoodPairing == null ? null : System.Text.Json.JsonSerializer.Serialize(dto.Characteristics.FoodPairing) },
        ReviewsCount = dto.Reviews?.Count ?? 0,
        AverageRating = dto.Reviews?.AverageRating ?? 0m,
        SelfLink = dto.Self?.Link
    };

    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
}).WithName("CreateProduct");

app.MapPut("/api/products/{id:int}", async (int id, CreateProductDto dto, ApplicationDbContext db) =>
{
    var existing = await db.Products.FindAsync(id);
    if (existing == null) return Results.NotFound();
    existing.Name = dto.Name;
    existing.Description = dto.Description;
    existing.Price = dto.Price;
    existing.DiscountPrice = dto.DiscountPrice;
    existing.Volume = dto.Volume;
    existing.Unit = dto.Unit;
    existing.AlcoholContent = dto.AlcoholContent;
    existing.Stock = dto.Stock;
    existing.IsAvailable = dto.IsAvailable;
    existing.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("UpdateProduct");

app.MapDelete("/api/products/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var existing = await db.Products.FindAsync(id);
    if (existing == null) return Results.NotFound();
    db.Products.Remove(existing);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("DeleteProduct");

app.MapPatch("/api/products/{id:int}", async (int id, JsonPatchDocument<Product> patchDoc, ApplicationDbContext db) =>
{
    if (patchDoc == null) return Results.BadRequest();
    var existing = await db.Products.FindAsync(id);
    if (existing == null) return Results.NotFound();

    // apply patch
    patchDoc.ApplyTo(existing);
    existing.UpdatedAt = DateTime.UtcNow;
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithName("PatchProduct");

// GraphQL endpoint
app.MapGraphQL("/graphql");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

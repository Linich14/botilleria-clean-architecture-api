using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using botilleria_clean_architecture_api;
using HotChocolate;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.DTOs;
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

// Configuración de autenticación JWT para proteger endpoints POST
// JWT Authentication setup to protect POST endpoints
var key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef"); // Clave de 256 bits para HMAC-SHA256
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Permitir HTTP en desarrollo
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, // No validar emisor en demo
            ValidateAudience = false, // No validar audiencia en demo
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false // No expirar tokens en demo
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

// Endpoint de autenticación para obtener token JWT
// Authentication endpoint to get JWT token
app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (request.Password == "secret123") // Contraseña demo
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(1), // Token válido por 1 hora
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return Results.Ok(new { Token = tokenString }); // Retornar token en JSON
    }
    return Results.Unauthorized(); // 401 si contraseña incorrecta
}).WithName("Login");

// Products endpoints
// Endpoint GET para listar todos los productos con relaciones incluidas
// GET endpoint to list all products with included relationships
app.MapGet("/api/products", async (ApplicationDbContext db) =>
{
    var products = await db.Products
        .Include(p => p.Category) // Incluir categoría
        .Include(p => p.Brand) // Incluir marca
        .Include(p => p.ProductType) // Incluir tipo de producto
        .Include(p => p.Origin!).ThenInclude(o => o.Country) // Incluir origen con país
        .Include(p => p.Origin!).ThenInclude(o => o.Region) // Incluir región
        .ToListAsync();
    
    var productDtos = products.Select(p => new botilleria_clean_architecture_api.DTOs.ProductDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        DiscountPrice = p.DiscountPrice,
        Volume = p.Volume ?? 0,
        Unit = p.Unit ?? "ml",
        AlcoholContent = p.AlcoholContent,
        Stock = p.Stock,
        IsAvailable = p.IsAvailable,
        CreatedAt = p.CreatedAt,
        UpdatedAt = p.UpdatedAt ?? DateTime.UtcNow,
        Vintage = null, // Not in current model
        Category = p.Category == null ? null : new botilleria_clean_architecture_api.DTOs.CategoryDto
        {
            Id = p.Category.Id,
            Name = p.Category.Name,
            Subcategory = p.Category.Subcategory
        },
        ProductType = p.ProductType == null ? null : new botilleria_clean_architecture_api.DTOs.ProductTypeDto
        {
            Id = p.ProductType.Id,
            Name = p.ProductType.Name
        },
        Brand = p.Brand == null ? null : new botilleria_clean_architecture_api.DTOs.BrandDto
        {
            Id = p.Brand.Id,
            Name = p.Brand.Name
        },
        Origin = p.Origin == null ? null : new botilleria_clean_architecture_api.DTOs.OriginDto
        {
            Id = p.Origin.Id,
            Country = p.Origin.Country == null ? null : new botilleria_clean_architecture_api.DTOs.CountryDto
            {
                Id = p.Origin.Country.Id,
                Name = p.Origin.Country.Name,
                IsoCode = p.Origin.Country.IsoCode
            },
            Region = p.Origin.Region == null ? null : new botilleria_clean_architecture_api.DTOs.RegionDto
            {
                Id = p.Origin.Region.Id,
                Name = p.Origin.Region.Name
            },
            Vineyard = p.Origin.Vineyard
        },
        Characteristics = p.Characteristics == null ? null : new botilleria_clean_architecture_api.DTOs.ProductCharacteristicsDto
        {
            Color = p.Characteristics.Color,
            Aroma = p.Characteristics.Aroma,
            Taste = p.Characteristics.Taste,
            ServingTemperature = p.Characteristics.ServingTemperature,
            FoodPairing = string.IsNullOrEmpty(p.Characteristics.FoodPairingJson) 
                ? new List<string>() 
                : System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Characteristics.FoodPairingJson) ?? new List<string>()
        }
    }).ToList();
    
    return Results.Ok(productDtos);
}).WithName("GetProducts");

// Endpoint GET para obtener un producto por ID (versión simplificada)
// GET endpoint to get a product by ID (simplified version)
app.MapGet("/api/products/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var product = await db.Products
        .Include(p => p.Category)
        .Include(p => p.Brand)
        .Include(p => p.ProductType)
        .Include(p => p.Origin!).ThenInclude(o => o.Country)
        .Include(p => p.Origin!).ThenInclude(o => o.Region)
        .FirstOrDefaultAsync(p => p.Id == id);
    
    if (product == null) return Results.NotFound(); // 404 si no existe
    
    var dto = new ProductDto
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        DiscountPrice = product.DiscountPrice,
        Volume = product.Volume ?? 0,
        Unit = product.Unit,
        AlcoholContent = product.AlcoholContent,
        Stock = product.Stock,
        IsAvailable = product.IsAvailable,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt ?? DateTime.UtcNow,
        Vintage = null, // Not in current model
        Category = product.Category == null ? null : new CategoryDto
        {
            Id = product.Category.Id,
            Name = product.Category.Name,
            Subcategory = product.Category.Subcategory
        },
        ProductType = product.ProductType == null ? null : new ProductTypeDto
        {
            Id = product.ProductType.Id,
            Name = product.ProductType.Name
        },
        Brand = product.Brand == null ? null : new BrandDto
        {
            Id = product.Brand.Id,
            Name = product.Brand.Name
        },
        Origin = product.Origin == null ? null : new OriginDto
        {
            Id = product.Origin.Id,
            Country = product.Origin.Country == null ? null : new CountryDto
            {
                Id = product.Origin.Country.Id,
                Name = product.Origin.Country.Name,
                IsoCode = product.Origin.Country.IsoCode
            },
            Region = product.Origin.Region == null ? null : new RegionDto
            {
                Id = product.Origin.Region.Id,
                Name = product.Origin.Region.Name
            },
            Vineyard = product.Origin.Vineyard
        },
        Characteristics = product.Characteristics == null ? null : new ProductCharacteristicsDto
        {
            Color = product.Characteristics.Color,
            Aroma = product.Characteristics.Aroma,
            Taste = product.Characteristics.Taste,
            ServingTemperature = product.Characteristics.ServingTemperature,
            FoodPairing = string.IsNullOrEmpty(product.Characteristics.FoodPairingJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(product.Characteristics.FoodPairingJson)
        }
    };
    return Results.Ok(dto); // Retornar DTO para evitar ciclos
}).WithName("GetProductById");

// Endpoint POST para crear un producto (requiere JWT)
// POST endpoint to create a product (requires JWT)
app.MapPost("/api/products", [Microsoft.AspNetCore.Authorization.Authorize] async (CreateProductDto dto, ApplicationDbContext db) =>
{
    // Lógica de upsert mínima similar a la mutación GraphQL
    // Minimal upsert logic similar to GraphQL mutation
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

// Categories endpoints
app.MapGet("/api/categories", async (ApplicationDbContext db) =>
{
    var categories = await db.Categories.ToListAsync();
    var dtos = categories.Select(c => new CategoryDto { Id = c.Id, Name = c.Name, Subcategory = c.Subcategory });
    return Results.Ok(dtos);
}).WithName("GetCategories");

app.MapGet("/api/categories/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var category = await db.Categories.FindAsync(id);
    if (category == null) return Results.NotFound();
    var dto = new CategoryDto { Id = category.Id, Name = category.Name, Subcategory = category.Subcategory };
    return Results.Ok(dto);
}).WithName("GetCategoryById");

app.MapPost("/api/categories", [Authorize] async (CreateCategoryDto dto, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name)) return Results.BadRequest("Name is required");

    var category = new Category { Name = dto.Name, Subcategory = dto.Subcategory };
    db.Categories.Add(category);
    await db.SaveChangesAsync();
    var resultDto = new CategoryDto { Id = category.Id, Name = category.Name, Subcategory = category.Subcategory };
    return Results.Created($"/api/categories/{category.Id}", resultDto);
}).WithName("CreateCategory");

// Brands endpoints
app.MapGet("/api/brands", async (ApplicationDbContext db) =>
{
    var brands = await db.Brands.ToListAsync();
    var dtos = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
    return Results.Ok(dtos);
}).WithName("GetBrands");

app.MapGet("/api/brands/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var brand = await db.Brands.FindAsync(id);
    if (brand == null) return Results.NotFound();
    var dto = new BrandDto { Id = brand.Id, Name = brand.Name };
    return Results.Ok(dto);
}).WithName("GetBrandById");

app.MapPost("/api/brands", [Authorize] async (CreateBrandDto dto, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name)) return Results.BadRequest("Name is required");

    var brand = new Brand { Name = dto.Name };
    db.Brands.Add(brand);
    await db.SaveChangesAsync();
    var resultDto = new BrandDto { Id = brand.Id, Name = brand.Name };
    return Results.Created($"/api/brands/{brand.Id}", resultDto);
}).WithName("CreateBrand");

// Product Types endpoints
app.MapGet("/api/producttypes", async (ApplicationDbContext db) =>
{
    var productTypes = await db.ProductTypes.ToListAsync();
    var dtos = productTypes.Select(pt => new ProductTypeDto { Id = pt.Id, Name = pt.Name });
    return Results.Ok(dtos);
}).WithName("GetProductTypes");

app.MapGet("/api/producttypes/{id:int}", async (int id, ApplicationDbContext db) =>
{
    var productType = await db.ProductTypes.FindAsync(id);
    if (productType == null) return Results.NotFound();
    var dto = new ProductTypeDto { Id = productType.Id, Name = productType.Name };
    return Results.Ok(dto);
}).WithName("GetProductTypeById");

app.MapPost("/api/producttypes", [Authorize] async (CreateProductTypeDto dto, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.Name)) return Results.BadRequest("Name is required");

    var productType = new ProductType { Name = dto.Name };
    db.ProductTypes.Add(productType);
    await db.SaveChangesAsync();
    var resultDto = new ProductTypeDto { Id = productType.Id, Name = productType.Name };
    return Results.Created($"/api/producttypes/{productType.Id}", resultDto);
}).WithName("CreateProductType");

// GraphQL endpoint
app.MapGraphQL("/graphql");

app.Run();

public record LoginRequest(string Password);

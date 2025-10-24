using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using botilleria_clean_architecture_api;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Infrastructure.Persistence;
using HotChocolate;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.DTOs;
using HotChocolate.AspNetCore;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configurar servicios base de la aplicación
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Soporte para operaciones complejas de JSON
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddHttpContextAccessor(); // Para acceder al contexto HTTP en servicios

// Proteger operaciones de escritura con tokens de acceso
var key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
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

// Consultas avanzadas y modificaciones de datos
builder.Services
    .AddGraphQLServer()
    .AddQueryType<botilleria_clean_architecture_api.GraphQL.Query>()
    .AddMutationType<botilleria_clean_architecture_api.Presentation.API.GraphQL.Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// Inyectar servicios para mantener el código desacoplado
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IOriginRepository, OriginRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductTypeService>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<RegionService>();
builder.Services.AddScoped<OriginService>();
builder.Services.AddScoped<AuditService>();

var app = builder.Build();

// Configurar el flujo de procesamiento de solicitudes
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

// Redirigir a HTTPS solo cuando sea necesario
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")) ||
    app.Configuration.GetValue<bool>("UseHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

// Verificar identidad del usuario antes de procesar solicitudes
app.UseAuthentication();
app.UseAuthorization();

// Punto de acceso para obtener credenciales de acceso
app.MapPost("/api/auth/login", (LoginRequest request) =>
{
    if (request.Password == "secret123")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return Results.Ok(new { Token = tokenString });
    }
    return Results.Unauthorized();
}).WithName("Login");

// Punto de acceso para consultas avanzadas
app.MapGraphQL("/graphql");

// Activar todos los controladores de la aplicación
app.MapControllers();

app.Run();

public record LoginRequest(string Password);

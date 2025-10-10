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
    .AddQueryType<botilleria_clean_architecture_api.Presentation.API.GraphQL.Query>()
    .AddMutationType<botilleria_clean_architecture_api.Presentation.API.GraphQL.Mutation>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// Dependency Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductTypeRepository, ProductTypeRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();
builder.Services.AddScoped<IOriginRepository, OriginRepository>();

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<BrandService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<ProductTypeService>();
builder.Services.AddScoped<CountryService>();
builder.Services.AddScoped<RegionService>();
builder.Services.AddScoped<OriginService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

// Solo usar HTTPS redirection si hay un puerto HTTPS configurado
// Only use HTTPS redirection if HTTPS port is configured
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT")) ||
    app.Configuration.GetValue<bool>("UseHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();// Endpoint de autenticación para obtener token JWT
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

// Minimal API endpoints removed - functionality moved to controllers for clean architecture
// GraphQL endpoint
app.MapGraphQL("/graphql");

app.MapControllers();

app.Run();

public record LoginRequest(string Password);

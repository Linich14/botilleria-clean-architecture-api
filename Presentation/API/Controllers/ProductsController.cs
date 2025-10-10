// Exponer operaciones de productos a través de HTTP
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Application.DTOs;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] GetProductsQuery query)
    {
        var products = await _productService.GetProductsAsync(query);
        var productDtos = products.Select(p => new ProductDto
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
            Category = p.Category == null ? null : new CategoryDto
            {
                Id = p.Category.Id,
                Name = p.Category.Name,
                Subcategory = p.Category.Subcategory
            },
            Brand = p.Brand == null ? null : new BrandDto
            {
                Id = p.Brand.Id,
                Name = p.Brand.Name
            },
            Origin = p.Origin == null ? null : new OriginDto
            {
                Id = p.Origin.Id,
                Country = p.Origin.Country == null ? null : new CountryDto
                {
                    Id = p.Origin.Country.Id,
                    Name = p.Origin.Country.Name,
                    IsoCode = p.Origin.Country.IsoCode
                },
                Region = p.Origin.Region == null ? null : new RegionDto
                {
                    Id = p.Origin.Region.Id,
                    Name = p.Origin.Region.Name
                },
                Vineyard = p.Origin.Vineyard
            },
            Characteristics = p.Characteristics == null ? null : new ProductCharacteristicsDto
            {
                Color = p.Characteristics.Color,
                Aroma = p.Characteristics.Aroma,
                Taste = p.Characteristics.Taste,
                ServingTemperature = p.Characteristics.ServingTemperature,
                FoodPairing = string.IsNullOrEmpty(p.Characteristics.FoodPairingJson) ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(p.Characteristics.FoodPairingJson)
            }
        });
        return Ok(productDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await _productService.GetProductAsync(new GetProductQuery { Id = id });
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProduct(CreateProductCommand command)
    {
        var product = await _productService.CreateProductAsync(command);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest();
        var product = await _productService.UpdateProductAsync(command);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(new DeleteProductCommand { Id = id });
        if (!result) return NotFound();
        return NoContent();
    }
}
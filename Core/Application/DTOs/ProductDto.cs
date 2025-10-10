
namespace botilleria_clean_architecture_api.Core.Application.DTOs;

// DTO para transferir datos de productos (evita referencias circulares)
// DTO for transferring product data (avoids circular references)
public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public decimal? DiscountPrice { get; init; }
    public int Volume { get; init; }
    public string Unit { get; init; } = "ml";
    public decimal? AlcoholContent { get; init; }
    public int Stock { get; init; }
    public bool IsAvailable { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int? Vintage { get; init; }
    
    public CategoryDto? Category { get; init; }
    public ProductTypeDto? ProductType { get; init; }
    public BrandDto? Brand { get; init; }
    public OriginDto? Origin { get; init; }
    public ProductCharacteristicsDto? Characteristics { get; init; }
}

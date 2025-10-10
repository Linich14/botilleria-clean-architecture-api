namespace botilleria_clean_architecture_api.Core.Application.DTOs;

// DTO para crear productos (entrada de datos)
// DTO for creating products (input data)
public class CreateProductDto
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? Volume { get; set; }
    public string? Unit { get; set; }
    public decimal? AlcoholContent { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public RelationDto? Category { get; set; }
    public RelationDto? Type { get; set; }
    public RelationDto? Brand { get; set; }
    public InputOriginDto? Origin { get; set; }
    public InputCharacteristicsDto? Characteristics { get; set; }
    public ReviewsDto? Reviews { get; set; }
    public SelfDto? Self { get; set; }
}

public class RelationDto { public int? Id { get; set; } public string? Name { get; set; } public string? Subcategory { get; set; } }
public class InputOriginDto { public RelationDto? Country { get; set; } public RelationDto? Region { get; set; } public string? Vineyard { get; set; } }
public class InputCharacteristicsDto { public string? Color { get; set; } public string? Aroma { get; set; } public string? Taste { get; set; } public string? ServingTemperature { get; set; } public string[]? FoodPairing { get; set; } }
public class ReviewsDto { public int Count { get; set; } public decimal AverageRating { get; set; } }
public class SelfDto { public string? Link { get; set; } }

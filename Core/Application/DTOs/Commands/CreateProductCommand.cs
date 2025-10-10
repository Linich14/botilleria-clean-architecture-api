namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class CreateProductCommand
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? Volume { get; set; }
    public string? Unit { get; set; }
    public decimal? AlcoholContent { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public int? CategoryId { get; set; }
    public int? ProductTypeId { get; set; }
    public int? BrandId { get; set; }
    public int? OriginId { get; set; }
    public string? Color { get; set; }
    public string? Aroma { get; set; }
    public string? Taste { get; set; }
    public string? ServingTemperature { get; set; }
    public string[]? FoodPairing { get; set; }
}
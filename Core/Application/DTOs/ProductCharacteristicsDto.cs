namespace botilleria_clean_architecture_api.Core.Application.DTOs;

public record ProductCharacteristicsDto
{
    public string? Color { get; init; }
    public string? Aroma { get; init; }
    public string? Taste { get; init; }
    public string? ServingTemperature { get; init; }
    public List<string> FoodPairing { get; init; } = new();
}

namespace botilleria_clean_architecture_api.DTOs;

public record RegionDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
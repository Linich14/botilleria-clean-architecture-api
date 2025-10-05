namespace botilleria_clean_architecture_api.DTOs;

public record OriginDto
{
    public int Id { get; init; }
    public CountryDto? Country { get; init; }
    public RegionDto? Region { get; init; }
    public string? Vineyard { get; init; }
}
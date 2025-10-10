namespace botilleria_clean_architecture_api.Core.Application.DTOs;

public record CountryDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string IsoCode { get; init; } = string.Empty;
}

namespace botilleria_clean_architecture_api.Core.Application.DTOs;

public record ProductTypeDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}

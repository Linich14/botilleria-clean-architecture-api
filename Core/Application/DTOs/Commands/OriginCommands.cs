using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class CreateOriginCommand
{
    public int CountryId { get; set; }
    public int RegionId { get; set; }
    public string? Vineyard { get; set; }
}

public class UpdateOriginCommand
{
    public int Id { get; set; }
    public int CountryId { get; set; }
    public int RegionId { get; set; }
    public string? Vineyard { get; set; }
}

public class DeleteOriginCommand
{
    public int Id { get; set; }
}
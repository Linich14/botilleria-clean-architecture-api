using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class CreateRegionCommand
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}

public class UpdateRegionCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}

public class DeleteRegionCommand
{
    public int Id { get; set; }
}
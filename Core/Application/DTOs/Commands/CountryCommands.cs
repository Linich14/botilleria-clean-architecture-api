using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class CreateCountryCommand
{
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
}

public class UpdateCountryCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IsoCode { get; set; } = string.Empty;
}

public class DeleteCountryCommand
{
    public int Id { get; set; }
}
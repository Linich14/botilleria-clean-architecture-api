using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class CreateCategoryCommand
{
    public string Name { get; set; } = string.Empty;
    public string? Subcategory { get; set; }
}

public class UpdateCategoryCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Subcategory { get; set; }
}

public class DeleteCategoryCommand
{
    public int Id { get; set; }
}
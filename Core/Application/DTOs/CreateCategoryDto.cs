namespace botilleria_clean_architecture_api.Core.Application.DTOs;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Subcategory { get; set; }
}

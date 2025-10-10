using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class CreateProductTypeCommand
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateProductTypeCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class DeleteProductTypeCommand
{
    public int Id { get; set; }
}
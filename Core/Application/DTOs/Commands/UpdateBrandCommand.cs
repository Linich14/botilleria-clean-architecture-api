namespace botilleria_clean_architecture_api.Core.Application.DTOs.Commands;

public class UpdateBrandCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
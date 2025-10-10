namespace botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

public class GetProductsQuery
{
    // Parámetros opcionales para filtrar
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public bool? IsAvailable { get; set; }
}
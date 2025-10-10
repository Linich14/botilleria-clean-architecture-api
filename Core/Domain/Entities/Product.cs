// Representar un producto de la botillería con sus características esenciales
using System.Text.Json.Serialization;

namespace botilleria_clean_architecture_api.Core.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public int? Volume { get; set; }
    public string? Unit { get; set; }
    public decimal? AlcoholContent { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public int? ProductTypeId { get; set; }
    public ProductType? ProductType { get; set; }
    public int? BrandId { get; set; }
    public Brand? Brand { get; set; }
    public int? OriginId { get; set; }
    public Origin? Origin { get; set; }
    public Characteristics? Characteristics { get; set; }
    public int ReviewsCount { get; set; }
    public decimal AverageRating { get; set; }
    public string? SelfLink { get; set; }
}

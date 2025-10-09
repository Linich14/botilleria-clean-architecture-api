using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using HotChocolate;

namespace botilleria_clean_architecture_api;

public class Origin
{
    public int Id { get; set; }
    
    public int CountryId { get; set; }
    public int RegionId { get; set; }
    
    [MaxLength(100)]
    public string? Vineyard { get; set; }
    
    // Navigation Properties
    public Country Country { get; set; } = null!;
    public Region Region { get; set; } = null!;
    
    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

[Owned]
public class Characteristics
{
    public string? Color { get; set; }
    public string? Aroma { get; set; }
    public string? Taste { get; set; }
    public string? ServingTemperature { get; set; }
    // stored as JSON string
    public string? FoodPairingJson { get; set; }
}

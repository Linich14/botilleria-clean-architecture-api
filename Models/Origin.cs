using Microsoft.EntityFrameworkCore;

namespace botilleria_clean_architecture_api;

public class Origin
{
    public int Id { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public int? RegionId { get; set; }
    public Region? Region { get; set; }
    public string? Vineyard { get; set; }
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

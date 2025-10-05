namespace botilleria_clean_architecture_api;

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? IsoCode { get; set; }
}

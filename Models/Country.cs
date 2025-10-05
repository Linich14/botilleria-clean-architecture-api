using System.ComponentModel.DataAnnotations;

namespace botilleria_clean_architecture_api;

public class Country
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2)]
    public string IsoCode { get; set; } = string.Empty;
    
    public ICollection<Region> Regions { get; set; } = new List<Region>();
    public ICollection<Origin> Origins { get; set; } = new List<Origin>();
}

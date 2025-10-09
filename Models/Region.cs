using System.ComponentModel.DataAnnotations;
using HotChocolate;

namespace botilleria_clean_architecture_api;

public class Region
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
    
    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Origin> Origins { get; set; } = new List<Origin>();
}

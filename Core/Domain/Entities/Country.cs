using System.ComponentModel.DataAnnotations;
using HotChocolate;

namespace botilleria_clean_architecture_api.Core.Domain.Entities;

public class Country
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(2)]
    public string IsoCode { get; set; } = string.Empty;
    
    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Region> Regions { get; set; } = new List<Region>();
    
    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Origin> Origins { get; set; } = new List<Origin>();
}

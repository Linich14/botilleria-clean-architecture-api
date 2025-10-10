using HotChocolate;

namespace botilleria_clean_architecture_api.Core.Domain.Entities;

public class ProductType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Product>? Products { get; set; }
}

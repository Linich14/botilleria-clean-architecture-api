using HotChocolate;

namespace botilleria_clean_architecture_api;

public class Brand
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Product>? Products { get; set; }
}

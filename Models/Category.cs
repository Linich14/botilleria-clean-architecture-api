using HotChocolate;

namespace botilleria_clean_architecture_api;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Subcategory { get; set; }

    [GraphQLIgnore] // Prevent infinite loops in GraphQL
    public ICollection<Product>? Products { get; set; }
}

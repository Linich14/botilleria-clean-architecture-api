namespace botilleria_clean_architecture_api;

public class ProductType
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Product>? Products { get; set; }
}

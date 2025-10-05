using HotChocolate;

namespace botilleria_clean_architecture_api.GraphQL;

public class Query
{
    public IQueryable<Product> GetProducts([Service] ApplicationDbContext db) => db.Products.AsQueryable();
}

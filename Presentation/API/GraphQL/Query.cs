using HotChocolate;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Infrastructure.Persistence;

namespace botilleria_clean_architecture_api.Presentation.API.GraphQL;

public class Query
{
    public IQueryable<Product> GetProducts([Service] ApplicationDbContext db) => db.Products.AsQueryable();
}

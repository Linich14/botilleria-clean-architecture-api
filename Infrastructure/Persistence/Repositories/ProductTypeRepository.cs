using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Infrastructure.Persistence;

namespace botilleria_clean_architecture_api.Infrastructure.Persistence.Repositories;

public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
{
    public ProductTypeRepository(ApplicationDbContext context) : base(context)
    {
    }
}
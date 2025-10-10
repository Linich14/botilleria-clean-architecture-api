using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    // Métodos específicos para Product si es necesario
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetProductsByBrandAsync(int brandId);
    Task<IEnumerable<Product>> GetAvailableProductsAsync();
}
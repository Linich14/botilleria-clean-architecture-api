// Coordinar transacciones múltiples manteniendo la consistencia de datos
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IBrandRepository Brands { get; }
    ICategoryRepository Categories { get; }
    IProductTypeRepository ProductTypes { get; }
    ICountryRepository Countries { get; }
    IRegionRepository Regions { get; }
    IOriginRepository Origins { get; }
    IAuditLogRepository AuditLogs { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
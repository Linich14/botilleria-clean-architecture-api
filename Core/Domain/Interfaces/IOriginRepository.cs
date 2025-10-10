using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Domain.Interfaces;

public interface IOriginRepository : IRepository<Origin>
{
    // Métodos específicos si es necesario
    Task<IEnumerable<Origin>> GetOriginsByCountryAsync(int countryId);
    Task<IEnumerable<Origin>> GetOriginsByRegionAsync(int regionId);
}
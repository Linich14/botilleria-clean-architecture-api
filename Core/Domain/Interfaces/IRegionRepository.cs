using botilleria_clean_architecture_api.Core.Domain.Entities;

namespace botilleria_clean_architecture_api.Core.Domain.Interfaces;

public interface IRegionRepository : IRepository<Region>
{
    // Métodos específicos si es necesario
    Task<IEnumerable<Region>> GetRegionsByCountryAsync(int countryId);
}
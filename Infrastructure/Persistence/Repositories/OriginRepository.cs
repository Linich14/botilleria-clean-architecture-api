using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace botilleria_clean_architecture_api.Infrastructure.Persistence.Repositories;

public class OriginRepository : Repository<Origin>, IOriginRepository
{
    public OriginRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Origin>> GetOriginsByCountryAsync(int countryId)
    {
        return await _dbSet.Where(o => o.CountryId == countryId).ToListAsync();
    }

    public async Task<IEnumerable<Origin>> GetOriginsByRegionAsync(int regionId)
    {
        return await _dbSet.Where(o => o.RegionId == regionId).ToListAsync();
    }
}
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace botilleria_clean_architecture_api.Infrastructure.Persistence.Repositories;

public class RegionRepository : Repository<Region>, IRegionRepository
{
    public RegionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Region>> GetRegionsByCountryAsync(int countryId)
    {
        return await _dbSet.Where(r => r.CountryId == countryId).ToListAsync();
    }
}
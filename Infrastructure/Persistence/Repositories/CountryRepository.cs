using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;
using botilleria_clean_architecture_api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace botilleria_clean_architecture_api.Infrastructure.Persistence.Repositories;

public class CountryRepository : Repository<Country>, ICountryRepository
{
    public CountryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Country?> GetByIsoCodeAsync(string isoCode)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.IsoCode == isoCode);
    }
}
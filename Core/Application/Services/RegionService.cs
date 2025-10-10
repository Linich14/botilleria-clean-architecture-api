using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class RegionService
{
    private readonly IUnitOfWork _unitOfWork;

    public RegionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Region> CreateRegionAsync(CreateRegionCommand command)
    {
        var country = await _unitOfWork.Countries.GetByIdAsync(command.CountryId);
        if (country == null)
        {
            throw new ArgumentException("Country not found");
        }

        var region = new Region
        {
            Name = command.Name,
            Country = country
        };

        await _unitOfWork.Regions.AddAsync(region);
        return region;
    }

    public async Task<Region?> UpdateRegionAsync(UpdateRegionCommand command)
    {
        var region = await _unitOfWork.Regions.GetByIdAsync(command.Id);
        if (region == null) return null;

        var country = await _unitOfWork.Countries.GetByIdAsync(command.CountryId);
        if (country == null)
        {
            throw new ArgumentException("Country not found");
        }

        region.Name = command.Name;
        region.Country = country;

        await _unitOfWork.Regions.UpdateAsync(region);
        return region;
    }

    public async Task<bool> DeleteRegionAsync(DeleteRegionCommand command)
    {
        var region = await _unitOfWork.Regions.GetByIdAsync(command.Id);
        if (region == null) return false;

        await _unitOfWork.Regions.DeleteAsync(region);
        return true;
    }

    public async Task<Region?> GetRegionAsync(GetRegionQuery query)
    {
        return await _unitOfWork.Regions.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Region>> GetRegionsAsync(GetRegionsQuery query)
    {
        return await _unitOfWork.Regions.GetAllAsync();
    }
}
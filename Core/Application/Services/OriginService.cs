using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class OriginService
{
    private readonly IUnitOfWork _unitOfWork;

    public OriginService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Origin> CreateOriginAsync(CreateOriginCommand command)
    {
        var country = await _unitOfWork.Countries.GetByIdAsync(command.CountryId);
        if (country == null)
        {
            throw new ArgumentException("Country not found");
        }

        var region = await _unitOfWork.Regions.GetByIdAsync(command.RegionId);
        if (region == null)
        {
            throw new ArgumentException("Region not found");
        }

        var origin = new Origin
        {
            Country = country,
            Region = region,
            Vineyard = command.Vineyard
        };

        await _unitOfWork.Origins.AddAsync(origin);
        return origin;
    }

    public async Task<Origin?> UpdateOriginAsync(UpdateOriginCommand command)
    {
        var origin = await _unitOfWork.Origins.GetByIdAsync(command.Id);
        if (origin == null) return null;

        var country = await _unitOfWork.Countries.GetByIdAsync(command.CountryId);
        if (country == null)
        {
            throw new ArgumentException("Country not found");
        }

        var region = await _unitOfWork.Regions.GetByIdAsync(command.RegionId);
        if (region == null)
        {
            throw new ArgumentException("Region not found");
        }

        origin.Country = country;
        origin.Region = region;
        origin.Vineyard = command.Vineyard;

        await _unitOfWork.Origins.UpdateAsync(origin);
        return origin;
    }

    public async Task<bool> DeleteOriginAsync(DeleteOriginCommand command)
    {
        var origin = await _unitOfWork.Origins.GetByIdAsync(command.Id);
        if (origin == null) return false;

        await _unitOfWork.Origins.DeleteAsync(origin);
        return true;
    }

    public async Task<Origin?> GetOriginAsync(GetOriginQuery query)
    {
        return await _unitOfWork.Origins.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Origin>> GetOriginsAsync(GetOriginsQuery query)
    {
        return await _unitOfWork.Origins.GetAllAsync();
    }
}
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class CountryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CountryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Country> CreateCountryAsync(CreateCountryCommand command)
    {
        var country = new Country
        {
            Name = command.Name,
            IsoCode = command.IsoCode
        };

        await _unitOfWork.Countries.AddAsync(country);
        return country;
    }

    public async Task<Country?> UpdateCountryAsync(UpdateCountryCommand command)
    {
        var country = await _unitOfWork.Countries.GetByIdAsync(command.Id);
        if (country == null) return null;

        country.Name = command.Name;
        country.IsoCode = command.IsoCode;

        await _unitOfWork.Countries.UpdateAsync(country);
        return country;
    }

    public async Task<bool> DeleteCountryAsync(DeleteCountryCommand command)
    {
        var country = await _unitOfWork.Countries.GetByIdAsync(command.Id);
        if (country == null) return false;

        await _unitOfWork.Countries.DeleteAsync(country);
        return true;
    }

    public async Task<Country?> GetCountryAsync(GetCountryQuery query)
    {
        return await _unitOfWork.Countries.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Country>> GetCountriesAsync(GetCountriesQuery query)
    {
        return await _unitOfWork.Countries.GetAllAsync();
    }
}
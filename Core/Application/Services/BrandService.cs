using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class BrandService
{
    private readonly IBrandRepository _brandRepository;

    public BrandService(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    public async Task<Brand> CreateBrandAsync(CreateBrandCommand command)
    {
        var brand = new Brand
        {
            Name = command.Name
        };

        await _brandRepository.AddAsync(brand);
        return brand;
    }

    public async Task<Brand?> UpdateBrandAsync(UpdateBrandCommand command)
    {
        var brand = await _brandRepository.GetByIdAsync(command.Id);
        if (brand == null) return null;

        brand.Name = command.Name;

        await _brandRepository.UpdateAsync(brand);
        return brand;
    }

    public async Task<bool> DeleteBrandAsync(DeleteBrandCommand command)
    {
        var brand = await _brandRepository.GetByIdAsync(command.Id);
        if (brand == null) return false;

        await _brandRepository.DeleteAsync(brand);
        return true;
    }

    public async Task<Brand?> GetBrandAsync(GetBrandQuery query)
    {
        return await _brandRepository.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Brand>> GetBrandsAsync(GetBrandsQuery query)
    {
        return await _brandRepository.GetAllAsync();
    }
}
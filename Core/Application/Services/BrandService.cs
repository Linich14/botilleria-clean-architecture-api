using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class BrandService
{
    private readonly IUnitOfWork _unitOfWork;

    public BrandService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Brand> CreateBrandAsync(CreateBrandCommand command)
    {
        var brand = new Brand
        {
            Name = command.Name
        };

        await _unitOfWork.Brands.AddAsync(brand);
        return brand;
    }

    public async Task<Brand?> UpdateBrandAsync(UpdateBrandCommand command)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(command.Id);
        if (brand == null) return null;

        brand.Name = command.Name;

        await _unitOfWork.Brands.UpdateAsync(brand);
        return brand;
    }

    public async Task<bool> DeleteBrandAsync(DeleteBrandCommand command)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(command.Id);
        if (brand == null) return false;

        await _unitOfWork.Brands.DeleteAsync(brand);
        return true;
    }

    public async Task<Brand?> GetBrandAsync(GetBrandQuery query)
    {
        return await _unitOfWork.Brands.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Brand>> GetBrandsAsync(GetBrandsQuery query)
    {
        return await _unitOfWork.Brands.GetAllAsync();
    }
}
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class ProductTypeService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductTypeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductType> CreateProductTypeAsync(CreateProductTypeCommand command)
    {
        var productType = new ProductType
        {
            Name = command.Name
        };

        await _unitOfWork.ProductTypes.AddAsync(productType);
        return productType;
    }

    public async Task<ProductType?> UpdateProductTypeAsync(UpdateProductTypeCommand command)
    {
        var productType = await _unitOfWork.ProductTypes.GetByIdAsync(command.Id);
        if (productType == null) return null;

        productType.Name = command.Name;

        await _unitOfWork.ProductTypes.UpdateAsync(productType);
        return productType;
    }

    public async Task<bool> DeleteProductTypeAsync(DeleteProductTypeCommand command)
    {
        var productType = await _unitOfWork.ProductTypes.GetByIdAsync(command.Id);
        if (productType == null) return false;

        await _unitOfWork.ProductTypes.DeleteAsync(productType);
        return true;
    }

    public async Task<ProductType?> GetProductTypeAsync(GetProductTypeQuery query)
    {
        return await _unitOfWork.ProductTypes.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<ProductType>> GetProductTypesAsync(GetProductTypesQuery query)
    {
        return await _unitOfWork.ProductTypes.GetAllAsync();
    }
}
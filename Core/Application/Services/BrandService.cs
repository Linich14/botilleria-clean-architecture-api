using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class BrandService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuditService _auditService;

    public BrandService(IUnitOfWork unitOfWork, AuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Brand> CreateBrandAsync(CreateBrandCommand command)
    {
        var brand = new Brand
        {
            Name = command.Name
        };

        await _unitOfWork.Brands.AddAsync(brand);

        // Registrar operación de creación para auditoría
        await _auditService.LogOperationAsync("CREATE", "Brand", brand.Id, null, brand);

        return brand;
    }

    public async Task<Brand?> UpdateBrandAsync(UpdateBrandCommand command)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(command.Id);
        if (brand == null) return null;

        // Capturar valores antiguos para auditoría
        var oldValues = new { brand.Name };

        brand.Name = command.Name;

        await _unitOfWork.Brands.UpdateAsync(brand);

        // Registrar operación de actualización para auditoría
        await _auditService.LogOperationAsync("UPDATE", "Brand", brand.Id, oldValues, brand);

        return brand;
    }

    public async Task<bool> DeleteBrandAsync(DeleteBrandCommand command)
    {
        var brand = await _unitOfWork.Brands.GetByIdAsync(command.Id);
        if (brand == null) return false;

        // Capturar valores para auditoría antes de eliminar
        var oldValues = new { brand.Name };

        await _unitOfWork.Brands.DeleteAsync(brand);

        // Registrar operación de eliminación para auditoría
        await _auditService.LogOperationAsync("DELETE", "Brand", command.Id, oldValues, null);

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
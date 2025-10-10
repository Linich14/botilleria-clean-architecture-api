using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AuditService _auditService;

    public CategoryService(IUnitOfWork unitOfWork, AuditService auditService)
    {
        _unitOfWork = unitOfWork;
        _auditService = auditService;
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryCommand command)
    {
        var category = new Category
        {
            Name = command.Name,
            Subcategory = command.Subcategory
        };

        await _unitOfWork.Categories.AddAsync(category);

        // Registrar operación de creación para auditoría
        await _auditService.LogOperationAsync("CREATE", "Category", category.Id, null, category);

        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(UpdateCategoryCommand command)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(command.Id);
        if (category == null) return null;

        // Capturar valores antiguos para auditoría
        var oldValues = new { category.Name, category.Subcategory };

        category.Name = command.Name;
        category.Subcategory = command.Subcategory;

        await _unitOfWork.Categories.UpdateAsync(category);

        // Registrar operación de actualización para auditoría
        await _auditService.LogOperationAsync("UPDATE", "Category", category.Id, oldValues, category);

        return category;
    }

    public async Task<bool> DeleteCategoryAsync(DeleteCategoryCommand command)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(command.Id);
        if (category == null) return false;

        // Capturar valores para auditoría antes de eliminar
        var oldValues = new { category.Name, category.Subcategory };

        await _unitOfWork.Categories.DeleteAsync(category);

        // Registrar operación de eliminación para auditoría
        await _auditService.LogOperationAsync("DELETE", "Category", command.Id, oldValues, null);

        return true;
    }

    public async Task<Category?> GetCategoryAsync(GetCategoryQuery query)
    {
        return await _unitOfWork.Categories.GetByIdAsync(query.Id);
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync(GetCategoriesQuery query)
    {
        return await _unitOfWork.Categories.GetAllAsync();
    }
}
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Domain.Entities;
using botilleria_clean_architecture_api.Core.Domain.Interfaces;

namespace botilleria_clean_architecture_api.Core.Application.Services;

public class CategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Category> CreateCategoryAsync(CreateCategoryCommand command)
    {
        var category = new Category
        {
            Name = command.Name,
            Subcategory = command.Subcategory
        };

        await _unitOfWork.Categories.AddAsync(category);
        return category;
    }

    public async Task<Category?> UpdateCategoryAsync(UpdateCategoryCommand command)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(command.Id);
        if (category == null) return null;

        category.Name = command.Name;
        category.Subcategory = command.Subcategory;

        await _unitOfWork.Categories.UpdateAsync(category);
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(DeleteCategoryCommand command)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(command.Id);
        if (category == null) return false;

        await _unitOfWork.Categories.DeleteAsync(category);
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
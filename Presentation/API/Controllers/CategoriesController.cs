using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _categoryService.GetCategoriesAsync(new GetCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryAsync(new GetCategoryQuery { Id = id });
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommand command)
    {
        var category = await _categoryService.CreateCategoryAsync(command);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory(int id, UpdateCategoryCommand command)
    {
        command.Id = id;
        var category = await _categoryService.UpdateCategoryAsync(command);
        if (category == null)
            return NotFound();

        return Ok(category);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(new DeleteCategoryCommand { Id = id });
        if (!result)
            return NotFound();

        return NoContent();
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductTypesController : ControllerBase
{
    private readonly ProductTypeService _productTypeService;

    public ProductTypesController(ProductTypeService productTypeService)
    {
        _productTypeService = productTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProductTypes()
    {
        var productTypes = await _productTypeService.GetProductTypesAsync(new GetProductTypesQuery());
        return Ok(productTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductType(int id)
    {
        var productType = await _productTypeService.GetProductTypeAsync(new GetProductTypeQuery { Id = id });
        if (productType == null)
            return NotFound();

        return Ok(productType);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateProductType(CreateProductTypeCommand command)
    {
        var productType = await _productTypeService.CreateProductTypeAsync(command);
        return CreatedAtAction(nameof(GetProductType), new { id = productType.Id }, productType);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateProductType(int id, UpdateProductTypeCommand command)
    {
        command.Id = id;
        var productType = await _productTypeService.UpdateProductTypeAsync(command);
        if (productType == null)
            return NotFound();

        return Ok(productType);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteProductType(int id)
    {
        var result = await _productTypeService.DeleteProductTypeAsync(new DeleteProductTypeCommand { Id = id });
        if (!result)
            return NotFound();

        return NoContent();
    }
}
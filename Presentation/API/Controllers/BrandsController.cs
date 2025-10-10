using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;
using botilleria_clean_architecture_api.Core.Application.DTOs;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly BrandService _brandService;

    public BrandsController(BrandService brandService)
    {
        _brandService = brandService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBrands()
    {
        var brands = await _brandService.GetBrandsAsync(new GetBrandsQuery());
        var brandDtos = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name });
        return Ok(brandDtos);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBrand(int id)
    {
        var brand = await _brandService.GetBrandAsync(new GetBrandQuery { Id = id });
        if (brand == null) return NotFound();
        return Ok(new BrandDto { Id = brand.Id, Name = brand.Name });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBrand(CreateBrandCommand command)
    {
        var brand = await _brandService.CreateBrandAsync(command);
        return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, new BrandDto { Id = brand.Id, Name = brand.Name });
    }

    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<IActionResult> UpdateBrand(int id, UpdateBrandCommand command)
    {
        if (id != command.Id) return BadRequest();
        var brand = await _brandService.UpdateBrandAsync(command);
        if (brand == null) return NotFound();
        return Ok(new BrandDto { Id = brand.Id, Name = brand.Name });
    }

    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var result = await _brandService.DeleteBrandAsync(new DeleteBrandCommand { Id = id });
        if (!result) return NotFound();
        return NoContent();
    }
}
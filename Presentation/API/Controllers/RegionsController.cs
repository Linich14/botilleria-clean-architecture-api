using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegionsController : ControllerBase
{
    private readonly RegionService _regionService;

    public RegionsController(RegionService regionService)
    {
        _regionService = regionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRegions()
    {
        var regions = await _regionService.GetRegionsAsync(new GetRegionsQuery());
        return Ok(regions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRegion(int id)
    {
        var region = await _regionService.GetRegionAsync(new GetRegionQuery { Id = id });
        if (region == null)
            return NotFound();

        return Ok(region);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateRegion(CreateRegionCommand command)
    {
        var region = await _regionService.CreateRegionAsync(command);
        return CreatedAtAction(nameof(GetRegion), new { id = region.Id }, region);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateRegion(int id, UpdateRegionCommand command)
    {
        command.Id = id;
        var region = await _regionService.UpdateRegionAsync(command);
        if (region == null)
            return NotFound();

        return Ok(region);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRegion(int id)
    {
        var result = await _regionService.DeleteRegionAsync(new DeleteRegionCommand { Id = id });
        if (!result)
            return NotFound();

        return NoContent();
    }
}
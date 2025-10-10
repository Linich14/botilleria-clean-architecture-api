using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OriginsController : ControllerBase
{
    private readonly OriginService _originService;

    public OriginsController(OriginService originService)
    {
        _originService = originService;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrigins()
    {
        var origins = await _originService.GetOriginsAsync(new GetOriginsQuery());
        return Ok(origins);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrigin(int id)
    {
        var origin = await _originService.GetOriginAsync(new GetOriginQuery { Id = id });
        if (origin == null)
            return NotFound();

        return Ok(origin);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateOrigin(CreateOriginCommand command)
    {
        var origin = await _originService.CreateOriginAsync(command);
        return CreatedAtAction(nameof(GetOrigin), new { id = origin.Id }, origin);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateOrigin(int id, UpdateOriginCommand command)
    {
        command.Id = id;
        var origin = await _originService.UpdateOriginAsync(command);
        if (origin == null)
            return NotFound();

        return Ok(origin);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteOrigin(int id)
    {
        var result = await _originService.DeleteOriginAsync(new DeleteOriginCommand { Id = id });
        if (!result)
            return NotFound();

        return NoContent();
    }
}
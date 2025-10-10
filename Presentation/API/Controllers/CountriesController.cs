using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using botilleria_clean_architecture_api.Core.Application.Services;
using botilleria_clean_architecture_api.Core.Application.DTOs.Commands;
using botilleria_clean_architecture_api.Core.Application.DTOs.Queries;

namespace botilleria_clean_architecture_api.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly CountryService _countryService;

    public CountriesController(CountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _countryService.GetCountriesAsync(new GetCountriesQuery());
        return Ok(countries);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCountry(int id)
    {
        var country = await _countryService.GetCountryAsync(new GetCountryQuery { Id = id });
        if (country == null)
            return NotFound();

        return Ok(country);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCountry(CreateCountryCommand command)
    {
        var country = await _countryService.CreateCountryAsync(command);
        return CreatedAtAction(nameof(GetCountry), new { id = country.Id }, country);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCountry(int id, UpdateCountryCommand command)
    {
        command.Id = id;
        var country = await _countryService.UpdateCountryAsync(command);
        if (country == null)
            return NotFound();

        return Ok(country);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var result = await _countryService.DeleteCountryAsync(new DeleteCountryCommand { Id = id });
        if (!result)
            return NotFound();

        return NoContent();
    }
}
using Microsoft.AspNetCore.Mvc;
using Modeer360.Services;

namespace Modeer360.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController : ControllerBase
{
    private readonly CountryService _countryService;

    public CountriesController(CountryService countryService)
    {
        _countryService = countryService;
    }

    [HttpPost("block")]
    public IActionResult BlockCountry([FromBody] BlockCountryRequest request)
    {
        var (success, message) = _countryService.AddBlockedCountry(
            request.CountryCode, request.CountryName);

        return success ? Ok(message) : Conflict(message);
    }

    [HttpDelete("block/{countryCode}")]
    public IActionResult UnblockCountry(string countryCode)
    {
        var (success, message) = _countryService.RemoveBlockedCountry(countryCode);
        return success ? Ok(message) : NotFound(message);
    }

    [HttpGet("blocked")]
    public IActionResult GetBlockedCountries(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var result = _countryService.GetBlockedCountries(page, pageSize, search);
        return Ok(result);
    }

    [HttpPost("temporal-block")]
    public IActionResult TemporalBlock([FromBody] TemporalBlockRequest request)
    {
        var (success, message) = _countryService.AddTemporalBlock(
            request.CountryCode, request.CountryName, request.DurationMinutes);

        if (!success && message.Contains("already"))
            return Conflict(message);

        return success ? Ok(message) : BadRequest(message);
    }
}

public record BlockCountryRequest(string CountryCode, string CountryName);
public record TemporalBlockRequest(string CountryCode, string CountryName, int DurationMinutes);
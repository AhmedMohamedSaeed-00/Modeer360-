using Microsoft.AspNetCore.Mvc;
using Modeer360.Models;
using Modeer360.Services;

namespace Modeer360.Controllers;

[ApiController]
[Route("api/ip")]
public class IpController : ControllerBase
{
    private readonly IpService _ipService;

    public IpController(IpService ipService)
    {
        _ipService = ipService;
    }

    [HttpGet("lookup")]
    public async Task<IActionResult> Lookup([FromQuery] string? ipAddress)
    {
        if (string.IsNullOrWhiteSpace(ipAddress))
            ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ipAddress) || !_ipService.IsValidIp(ipAddress))
            return BadRequest("Invalid or missing IP address.");

        var result = await _ipService.LookupIpAsync(ipAddress);
        return result != null ? Ok(result) : StatusCode(503, "Could not fetch IP info.");
    }

    [HttpGet("check-block")]
    public async Task<IActionResult> CheckBlock()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrWhiteSpace(ipAddress))
            return BadRequest("Could not determine caller IP.");

        var (result, isBlocked) = await _ipService.CheckBlockAsync(ipAddress);

        if (result == null)
            return StatusCode(503, "Could not fetch IP info.");

        _ipService.AddLog(new BlockAttemptLog
        {
            IpAddress = ipAddress,
            CountryCode = result.CountryCode,
            IsBlocked = isBlocked,
            UserAgent = Request.Headers.UserAgent.ToString()
        });

        return Ok(new
        {
            result.IpAddress,
            result.CountryCode,
            result.CountryName,
            IsBlocked = isBlocked
        });
    }
}
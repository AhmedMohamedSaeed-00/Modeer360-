using Microsoft.AspNetCore.Mvc;
using Modeer360.Services;

namespace Modeer360.Controllers;

[ApiController]
[Route("api/logs")]
public class LogsController : ControllerBase
{
    private readonly IpService _ipService;

    public LogsController(IpService ipService)
    {
        _ipService = ipService;
    }

    [HttpGet("blocked-attempts")]
    public IActionResult GetBlockedAttempts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var logs = _ipService.GetLogs(page, pageSize);
        return Ok(logs);
    }
}
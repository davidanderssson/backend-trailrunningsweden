using Microsoft.AspNetCore.Mvc;
using TRS_API.Middleware;

namespace TRS_API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowWithoutApiKey]
[Tags("Health")]
public class HealthController : ControllerBase
{
    /// <summary>Checks that the API is running.</summary>
    /// <remarks>Does not require an API key. Used by Railway's healthcheck.</remarks>
    /// <response code="200">The API is running.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok();
    }
}

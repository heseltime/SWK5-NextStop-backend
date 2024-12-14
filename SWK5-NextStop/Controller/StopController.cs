namespace SWK5_NextStop.Controllers;

using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.DAL;

[ApiController]
[Route("api/[controller]")]
public class StopController : ControllerBase
{
    private readonly StopRepository _stopRepository;

    public StopController(StopRepository stopRepository)
    {
        _stopRepository = stopRepository;
    }

    /// <summary>
    /// Retrieves all stops.
    /// </summary>
    /// <returns>A list of stops.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllStops()
    {
        var stops = await _stopRepository.GetAllStopsAsync();

        if (stops == null || !stops.Any())
        {
            return NotFound("No stops found.");
        }

        return Ok(stops);
    }
}
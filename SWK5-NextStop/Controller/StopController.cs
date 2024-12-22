using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.Service;
using SWK5_NextStop.DTO;

namespace SWK5_NextStop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StopController : ControllerBase
{
    private readonly StopService _stopService;

    public StopController(StopService stopService)
    {
        _stopService = stopService;
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
        var stops = await _stopService.GetAllStopsAsync();

        if (stops == null || !stops.Any())
        {
            return NotFound("No stops found.");
        }

        return Ok(stops);
    }

    /// <summary>
    /// Adds a new stop.
    /// </summary>
    /// <param name="stopDto">The stop to add.</param>
    /// <returns>The created stop.</returns>
    [HttpPost]
    [Authorize] 
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddStop([FromBody] StopDTO stopDto)
    {
        if (stopDto == null || string.IsNullOrWhiteSpace(stopDto.Name))
        {
            return BadRequest("Invalid stop data.");
        }

        var createdStop = await _stopService.AddStopAsync(stopDto);

        return CreatedAtAction(nameof(GetAllStops), new { id = createdStop.StopId }, createdStop);
    }

    /// <summary>
    /// Updates an existing stop.
    /// </summary>
    /// <param name="id">The ID of the stop to update.</param>
    /// <param name="stopDto">The updated stop data.</param>
    /// <returns>A status indicating success or failure.</returns>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStop(int id, [FromBody] StopDTO stopDto)
    {
        if (stopDto == null || id <= 0 || string.IsNullOrWhiteSpace(stopDto.Name))
        {
            return BadRequest("Invalid stop data.");
        }

        var existingStop = await _stopService.GetStopByIdAsync(id);

        if (existingStop == null)
        {
            return NotFound($"Stop with ID {id} not found.");
        }

        await _stopService.UpdateStopAsync(id, stopDto);

        return NoContent(); // Indicates success with no response body
    }
    
    /// <summary>
    /// Searches for stops by name, partial name, or GPS coordinates.
    /// </summary>
    /// <param name="query">The search query (name or part of name).</param>
    /// <param name="latitude">The latitude for GPS search (optional).</param>
    /// <param name="longitude">The longitude for GPS search (optional).</param>
    /// <returns>A list of matching stops.</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchStops(string? query, double? latitude, double? longitude)
    {
        var stops = await _stopService.SearchStopsAsync(query, latitude, longitude);

        if (stops == null || !stops.Any())
        {
            return NotFound("No stops match the search criteria.");
        }

        return Ok(stops);
    }

}

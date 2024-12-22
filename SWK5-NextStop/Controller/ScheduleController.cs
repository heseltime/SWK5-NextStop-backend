using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.Service;
using SWK5_NextStop.DTO;

namespace SWK5_NextStop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ScheduleService _scheduleService;

    public ScheduleController(ScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>
    /// Creates a new schedule.
    /// </summary>
    /// <param name="scheduleDto">The schedule to create.</param>
    /// <returns>The created schedule.</returns>
    [HttpPost]
    [Authorize] // Requires authentication
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddSchedule([FromBody] ScheduleDTO scheduleDto)
    {
        if (scheduleDto == null || scheduleDto.RouteId <= 0)
        {
            return BadRequest("Invalid schedule data.");
        }

        var createdSchedule = await _scheduleService.AddScheduleAsync(scheduleDto);

        return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.ScheduleId }, createdSchedule);
    }

    /// <summary>
    /// Retrieves a schedule by its ID.
    /// </summary>
    /// <param name="id">The ID of the schedule to retrieve.</param>
    /// <returns>The requested schedule.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScheduleById(int id)
    {
        var schedule = await _scheduleService.GetScheduleByIdAsync(id);

        if (schedule == null)
        {
            return NotFound($"Schedule with ID {id} not found.");
        }

        return Ok(schedule);
    }

    /// <summary>
    /// Retrieves all schedules.
    /// </summary>
    /// <returns>A list of all schedules.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllSchedules()
    {
        var schedules = await _scheduleService.GetAllSchedulesAsync();

        if (schedules == null || !schedules.Any())
        {
            return NotFound("No schedules found.");
        }

        return Ok(schedules);
    }
    
    [HttpGet("betweenStops")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindSchedulesBetweenStops(int startStopId, int endStopId)
    {
        var schedules = await _scheduleService.FindSchedulesBetweenStopsAsync(startStopId, endStopId);

        if (schedules == null || !schedules.Any())
        {
            return NotFound($"No schedules found between stops {startStopId} and {endStopId}.");
        }

        return Ok(schedules);
    }
    
    [HttpGet("byTime")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FindSchedulesByTime(int startStopId, int endStopId, [FromQuery] string startTime, [FromQuery] string arrivalTime)
    {
        if (!TimeOnly.TryParse(startTime, out var parsedStartTime) || !TimeOnly.TryParse(arrivalTime, out var parsedArrivalTime))
        {
            return BadRequest("Invalid time format. Use HH:mm.");
        }

        var schedules = await _scheduleService.FindSchedulesByTimeAsync(startStopId, endStopId, parsedStartTime, parsedArrivalTime);

        if (schedules == null || !schedules.Any())
        {
            return NotFound($"No schedules found between stops {startStopId} and {endStopId} within the given time range.");
        }

        return Ok(schedules);
    }
    
    [HttpGet("nextConnections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetNextConnections(
        [FromQuery] int stopId,
        [FromQuery] DateTime? dateTime,
        [FromQuery] int count = 5)
    {
        if (stopId <= 0)
        {
            return BadRequest("Invalid stop ID.");
        }

        var dateTimeToUse = dateTime ?? DateTime.Now;

        var connections = await _scheduleService.GetNextConnectionsAsync(stopId, dateTimeToUse, count);

        if (connections == null || !connections.Any())
        {
            return NotFound($"No connections found for stop ID {stopId} starting from {dateTimeToUse}.");
        }

        return Ok(connections);
    }
    
    [HttpPost("checkIn")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CheckIn([FromBody] CheckInDTO checkInDto)
    {
        try
        {
            var checkInId = await _scheduleService.HandleCheckInAsync(checkInDto);

            return CreatedAtAction(nameof(CheckIn), new { id = checkInId }, new { CheckInId = checkInId });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized("Invalid API key.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the check-in.");
        }
    }

}

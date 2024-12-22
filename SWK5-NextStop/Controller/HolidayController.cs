using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.Service;
using SWK5_NextStop.DTO;

namespace SWK5_NextStop.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HolidayController : ControllerBase
{
    private readonly HolidayService _holidayService;

    public HolidayController(HolidayService holidayService)
    {
        _holidayService = holidayService;
    }

    /// <summary>
    /// Retrieves all holidays.
    /// </summary>
    /// <returns>A list of holidays.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllHolidays()
    {
        var holidays = await _holidayService.GetAllHolidaysAsync();

        if (holidays == null || !holidays.Any())
        {
            return NotFound("No holidays found.");
        }

        return Ok(holidays);
    }

    /// <summary>
    /// Adds a new holiday.
    /// </summary>
    /// <param name="holidayDto">The holiday to add.</param>
    /// <returns>The created holiday.</returns>
    [HttpPost]
    [Authorize] // Requires authentication
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddHoliday([FromBody] HolidayDTO holidayDto)
    {
        if (holidayDto == null || string.IsNullOrWhiteSpace(holidayDto.Description))
        {
            return BadRequest("Invalid holiday data.");
        }

        var createdHoliday = await _holidayService.AddHolidayAsync(holidayDto);

        return CreatedAtAction(nameof(GetAllHolidays), new { id = createdHoliday.Id }, createdHoliday);
    }

    /// <summary>
    /// Updates an existing holiday.
    /// </summary>
    /// <param name="id">The ID of the holiday to update.</param>
    /// <param name="holidayDto">The updated holiday data.</param>
    /// <returns>A status indicating success or failure.</returns>
    [HttpPut("{id}")]
    [Authorize] // Requires authentication
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateHoliday(int id, [FromBody] HolidayDTO holidayDto)
    {
        if (holidayDto == null || id <= 0 || string.IsNullOrWhiteSpace(holidayDto.Description))
        {
            return BadRequest("Invalid holiday data.");
        }

        var existingHoliday = await _holidayService.GetHolidayByIdAsync(id);

        if (existingHoliday == null)
        {
            return NotFound($"Holiday with ID {id} not found.");
        }

        await _holidayService.UpdateHolidayAsync(id, holidayDto);

        return NoContent(); // Indicates success with no response body
    }
}

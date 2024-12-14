namespace SWK5_NextStop.Controllers;

using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.Service;

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
}
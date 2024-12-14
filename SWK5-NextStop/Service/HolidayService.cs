using SWK5_NextStop.DAL;
using SWK5_NextStop.DTO;
using SWK5_NextStop.Mapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWK5_NextStop.Service;

public class HolidayService
{
    private readonly HolidayRepository _holidayRepository;

    public HolidayService(HolidayRepository holidayRepository)
    {
        _holidayRepository = holidayRepository;
    }

    /// <summary>
    /// Retrieves all holidays with optional business logic.
    /// </summary>
    /// <returns>A list of holiday DTOs with additional logic applied.</returns>
    public async Task<IEnumerable<HolidayDTO>> GetAllHolidaysAsync()
    {
        var holidays = await _holidayRepository.GetAllHolidaysAsync();

        // Business logic example: Filter holidays and map to DTOs
        return holidays
            .Where(h => h.Date >= DateTime.Now) // Example logic: only future holidays
            .Select(HolidayMapper.ToDTO)
            .ToList();
    }
}
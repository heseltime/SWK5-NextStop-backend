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

    /// <summary>
    /// Adds a new holiday to the database.
    /// </summary>
    /// <param name="holidayDto">The holiday DTO to add.</param>
    /// <returns>The added holiday DTO.</returns>
    public async Task<HolidayDTO> AddHolidayAsync(HolidayDTO holidayDto)
    {
        if (holidayDto == null)
        {
            throw new ArgumentNullException(nameof(holidayDto), "Holiday DTO cannot be null.");
        }

        // Map DTO to entity
        var holidayEntity = HolidayMapper.ToDomain(holidayDto);

        // Save to repository
        bool success = await _holidayRepository.CreateHolidayAsync(holidayEntity);
        if (!success)
        {
            throw new Exception("Failed to create holiday.");
        }

        // Map entity back to DTO and return
        return HolidayMapper.ToDTO(holidayEntity);
    }


    /// <summary>
    /// Updates an existing holiday.
    /// </summary>
    /// <param name="id">The ID of the holiday to update.</param>
    /// <param name="holidayDto">The updated holiday DTO.</param>
    /// <returns>Task representing the asynchronous operation.</returns>
    public async Task UpdateHolidayAsync(int id, HolidayDTO holidayDto)
    {
        if (holidayDto == null)
        {
            throw new ArgumentNullException(nameof(holidayDto), "Holiday DTO cannot be null.");
        }

        var existingHoliday = await _holidayRepository.GetHolidayByIdAsync(id);
        if (existingHoliday == null)
        {
            throw new KeyNotFoundException($"Holiday with ID {id} not found.");
        }

        // Map updated DTO to entity
        var updatedHolidayEntity = HolidayMapper.ToDomain(holidayDto);
        updatedHolidayEntity.Id = id; // Ensure the ID matches

        // Update repository
        bool success = await _holidayRepository.UpdateHolidayAsync(updatedHolidayEntity);
        if (!success)
        {
            throw new Exception("Failed to update holiday.");
        }
    }


    /// <summary>
    /// Retrieves a holiday by ID.
    /// </summary>
    /// <param name="id">The ID of the holiday to retrieve.</param>
    /// <returns>The holiday DTO, or null if not found.</returns>
    public async Task<HolidayDTO?> GetHolidayByIdAsync(int id)
    {
        var holiday = await _holidayRepository.GetHolidayByIdAsync(id);
        return holiday != null ? HolidayMapper.ToDTO(holiday) : null;
    }
}

namespace SWK5_NextStop.Service;

using SWK5_NextStop.DAL;
using SWK5_NextStop.DTO;
using SWK5_NextStop.Mapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class StopService
{
    private readonly StopRepository _stopRepository;

    public StopService(StopRepository stopRepository)
    {
        _stopRepository = stopRepository;
    }

    /// <summary>
    /// Retrieves all stops with optional business logic.
    /// </summary>
    /// <returns>A list of stop DTOs.</returns>
    public async Task<IEnumerable<StopDTO>> GetAllStopsAsync()
    {
        var stops = await _stopRepository.GetAllStopsAsync();

        // Example business logic: Filter or transform stops if needed
        return stops
            .Select(StopMapper.ToDTO)
            .ToList();
    }
    
    /// <summary>
    /// Retrieves a stop by id
    /// </summary>
    /// <returns>A stop DTO.</returns>
    public async Task<StopDTO> GetStopAsync(int id)
    {
        var stop = await _stopRepository.GetStopByIdAsync(id);

        // Example business logic: Filter or transform stops if needed
        return StopMapper.ToDTO(stop);
    }

    /// <summary>
    /// Adds a new stop.
    /// </summary>
    /// <param name="stopDto">The stop to add.</param>
    /// <returns>The created stop DTO.</returns>
    public async Task<StopDTO> AddStopAsync(StopDTO stopDto)
    {
        if (stopDto == null)
        {
            throw new ArgumentNullException(nameof(stopDto), "Stop DTO cannot be null.");
        }

        // Map DTO to domain entity
        var stopEntity = StopMapper.ToDomain(stopDto);

        // Save to repository and retrieve the result
        var isCreated = await _stopRepository.CreateStopAsync(stopEntity);

        // Map the created entity to DTO and return
        return StopMapper.ToDTO(stopEntity);
    }

    /// <summary>
    /// Updates an existing stop.
    /// </summary>
    /// <param name="id">The ID of the stop to update.</param>
    /// <param name="stopDto">The updated stop data.</param>
    public async Task UpdateStopAsync(int id, StopDTO stopDto)
    {
        if (stopDto == null)
        {
            throw new ArgumentNullException(nameof(stopDto), "Stop DTO cannot be null.");
        }

        // Retrieve the existing stop
        var existingStop = await _stopRepository.GetStopByIdAsync(id);
        if (existingStop == null)
        {
            throw new KeyNotFoundException($"Stop with ID {id} not found.");
        }

        // Map updated DTO to entity
        var updatedStop = StopMapper.ToDomain(stopDto);
        updatedStop.StopId = id; // Ensure the ID is set correctly

        // Update repository
        await _stopRepository.UpdateStopAsync(updatedStop);
    }

    /// <summary>
    /// Retrieves a stop by its ID.
    /// </summary>
    /// <param name="id">The ID of the stop to retrieve.</param>
    /// <returns>The stop DTO, or null if not found.</returns>
    public async Task<StopDTO?> GetStopByIdAsync(int id)
    {
        var stop = await _stopRepository.GetStopByIdAsync(id);
        return stop != null ? StopMapper.ToDTO(stop) : null;
    }
    
    public async Task<IEnumerable<StopDTO>> SearchStopsAsync(string? query, double? latitude, double? longitude)
    {
        var stops = await _stopRepository.SearchStopsAsync(query, latitude, longitude);

        // Map the results to DTOs
        return stops.Select(StopMapper.ToDTO);
    }
}

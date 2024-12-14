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
}
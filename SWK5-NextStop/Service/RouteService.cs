namespace SWK5_NextStop.Service;

using SWK5_NextStop.DAL;
using SWK5_NextStop.DTO;
using SWK5_NextStop.Mapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RouteService
{
    private readonly RouteRepository _routeRepository;

    public RouteService(RouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    /// <summary>
    /// Retrieves all routes with optional business logic.
    /// </summary>
    /// <returns>A list of route DTOs.</returns>
    public async Task<IEnumerable<RouteDTO>> GetAllRoutesAsync()
    {
        var routes = await _routeRepository.GetAllRoutesAsync();

        // Example business logic: Filter by validity or other conditions
        return routes
            .Where(r => r.ValidityPeriod != null) // Example logic: only valid routes
            .Select(RouteMapper.ToDTO)
            .ToList();
    }
}
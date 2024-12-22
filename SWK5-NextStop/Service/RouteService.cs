using NextStop.Data;

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
    private readonly StopRepository _stopRepository;

    public RouteService(RouteRepository routeRepository, StopRepository stopRepository)
    {
        _routeRepository = routeRepository;
        _stopRepository = stopRepository;
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
    
    public async Task<RouteDTO?> GetRouteByIdAsync(int routeId)
    {
        var route = await _routeRepository.GetRouteByIdAsync(routeId);
        return route != null ? RouteMapper.ToDTO(route) : null;
    }
    
    public async Task<IEnumerable<RouteDTO>> SearchRoutesAsync(string? routeNumber, string? validityPeriod)
    {
        var routes = await _routeRepository.SearchRoutesAsync(routeNumber, validityPeriod);
        return routes.Select(RouteMapper.ToDTO);
    }
    
    public async Task<RouteDTO> AddRouteAsync(RouteDTO routeDto)
    {
        if (routeDto == null)
        {
            throw new ArgumentNullException(nameof(routeDto), "Route DTO cannot be null.");
        }

        // Map DTO to domain entity
        var routeEntity = RouteMapper.ToDomain(routeDto);

        // Save to repository and retrieve the created route
        var createdRoute = await _routeRepository.CreateRouteAsync(routeEntity);
        
        // INDEPENDENTLY add the contained RouteStops
        foreach (var routeStopDto in routeDto.RouteStops)
        {
            // Check if the stop exists
            var existingStop = await _stopRepository.GetStopByIdAsync(routeStopDto.StopId);
            if (existingStop == null)
            {
                // Stops need to be added separately
                throw new ArgumentNullException(nameof(routeStopDto), "Route Stop DTOs need to refer to existing stop.");
            }

            // Add the stop to the route's list in the database
            await _routeRepository.AddRouteStopAsync(createdRoute.RouteId, routeStopDto.StopId, routeStopDto.SequenceNumber);
        }

        // Map the created entity back to DTO
        return RouteMapper.ToDTO(createdRoute);
    }
    
    public async Task UpdateRouteAsync(int id, RouteDTO routeDto)
    {
        if (routeDto == null)
        {
            throw new ArgumentNullException(nameof(routeDto), "Route DTO cannot be null.");
        }

        var existingRoute = await _routeRepository.GetRouteByIdAsync(id);

        if (existingRoute == null)
        {
            throw new KeyNotFoundException($"Route with ID {id} not found.");
        }

        // Map updated DTO to entity
        var updatedRoute = RouteMapper.ToDomain(routeDto);
        updatedRoute.RouteId = id; // Ensure the ID matches

        // Update in repository
        await _routeRepository.UpdateRouteAsync(updatedRoute);
    }
}
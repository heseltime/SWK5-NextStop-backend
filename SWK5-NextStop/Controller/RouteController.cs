namespace SWK5_NextStop.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.Service;
using SWK5_NextStop.DTO;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly RouteService _routeService;

    public RouteController(RouteService routeService)
    {
        _routeService = routeService;
    }

    /// <summary>
    /// Retrieves all routes.
    /// </summary>
    /// <returns>A list of routes.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllRoutes()
    {
        var routes = await _routeService.GetAllRoutesAsync();

        if (routes == null || !routes.Any())
        {
            return NotFound("No routes found.");
        }

        return Ok(routes);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRouteById(int id)
    {
        var route = await _routeService.GetRouteByIdAsync(id);

        if (route == null)
        {
            return NotFound($"Route with ID {id} not found.");
        }

        return Ok(route);
    }
    
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SearchRoutes(string? routeNumber, string? validityPeriod)
    {
        var routes = await _routeService.SearchRoutesAsync(routeNumber, validityPeriod);

        if (routes == null || !routes.Any())
        {
            return NotFound("No routes match the search criteria.");
        }

        return Ok(routes);
    }
    
    /// <summary>
    /// Adds a new route.
    /// </summary>
    /// <param name="routeDto">The route to add.</param>
    /// <returns>The created route.</returns>
    [HttpPost]
    [Authorize] // Requires authentication
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRoute([FromBody] RouteDTO routeDto)
    {
        if (routeDto == null || string.IsNullOrWhiteSpace(routeDto.RouteNumber))
        {
            return BadRequest("Invalid route data.");
        }

        try
        {
            var createdRoute = await _routeService.AddRouteAsync(routeDto);
            return CreatedAtAction(nameof(GetRouteById), new { id = createdRoute.RouteId }, createdRoute);
        }
        catch
        {
            return BadRequest("Failed to add route: RouteDTO null or RouteDTO Stop doesn't exist.");
        }
    }

    /// <summary>
    /// Updates an existing route.
    /// </summary>
    /// <param name="id">The ID of the route to update.</param>
    /// <param name="routeDto">The updated route data.</param>
    /// <returns>A status indicating success or failure.</returns>
    [HttpPut("{id}")]
    [Authorize] // Requires authentication
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateRoute(int id, [FromBody] RouteDTO routeDto)
    {
        if (routeDto == null || id <= 0 || string.IsNullOrWhiteSpace(routeDto.RouteNumber))
        {
            return BadRequest("Invalid route data.");
        }

        var existingRoute = await _routeService.GetRouteByIdAsync(id);

        if (existingRoute == null)
        {
            return NotFound($"Route with ID {id} not found.");
        }

        await _routeService.UpdateRouteAsync(id, routeDto);

        return NoContent(); // Indicates success with no response body
    }
}
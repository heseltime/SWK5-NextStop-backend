namespace SWK5_NextStop.Controllers;

using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.DAL;

[ApiController]
[Route("api/[controller]")]
public class RouteController : ControllerBase
{
    private readonly RouteRepository _routeRepository;

    public RouteController(RouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
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
        var routes = await _routeRepository.GetAllRoutesAsync();

        if (routes == null || !routes.Any())
        {
            return NotFound("No routes found.");
        }

        return Ok(routes);
    }
}
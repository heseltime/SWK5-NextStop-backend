namespace SWK5_NextStop.Controllers;

using Microsoft.AspNetCore.Mvc;
using SWK5_NextStop.Service;

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
}
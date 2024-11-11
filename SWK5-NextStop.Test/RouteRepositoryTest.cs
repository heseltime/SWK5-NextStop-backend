using Xunit;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Threading.Tasks;
using System.Linq;

public class RouteRepositoryIntegrationTest
{
    // Connection string for testing
    private readonly string _testConnectionString = "Host=localhost;Port=5432;Database=next_stop;Username=postgres;Password=";
    private readonly RouteRepository _routeRepository;

    public RouteRepositoryIntegrationTest()
    {
        var connectionFactory = new ConnectionFactory(_testConnectionString);
        _routeRepository = new RouteRepository(connectionFactory);
    }

    [Fact]
    public async Task GetAllRoutesAsync_ShouldReturnRoutesFromDatabase()
    {
        // Act
        var result = await _routeRepository.GetAllRoutesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any()); // Ensure there is data in the test database
    }

    [Fact]
    public async Task CreateRouteAsync_ShouldAddRouteToDatabase()
    {
        // Arrange
        var uniqueRouteNumber = "TestRoute_" + System.Guid.NewGuid().ToString();
        var newRoute = new Route
        {
            RouteNumber = uniqueRouteNumber,
            ValidityPeriod = "2025-2026",
            DayValidity = "Weekends",
            CompanyId = 1
        };

        try
        {
            // Act
            var result = await _routeRepository.CreateRouteAsync(newRoute);

            // Assert
            Assert.True(result);

            // Verify the route was added
            var createdRoute = (await _routeRepository.GetAllRoutesAsync())
                .FirstOrDefault(r => r.RouteNumber == uniqueRouteNumber);

            Assert.NotNull(createdRoute);
            Assert.Equal(uniqueRouteNumber, createdRoute.RouteNumber);
        }
        finally
        {
            // Clean up to maintain database state
            var createdRoute = (await _routeRepository.GetAllRoutesAsync())
                .FirstOrDefault(r => r.RouteNumber == uniqueRouteNumber);

            if (createdRoute != null)
            {
                await _routeRepository.DeleteRouteAsync(createdRoute.RouteId);
            }
        }
    }

    [Fact]
    public async Task UpdateRouteAsync_ShouldUpdateRouteInDatabase()
    {
        // Arrange
        var uniqueRouteNumber = "TestRouteUpdate_" + System.Guid.NewGuid().ToString();
        var newRoute = new Route
        {
            RouteNumber = uniqueRouteNumber,
            ValidityPeriod = "2025-2026",
            DayValidity = "Weekends",
            CompanyId = 1
        };

        try
        {
            // Create a new route to update
            await _routeRepository.CreateRouteAsync(newRoute);
            var createdRoute = (await _routeRepository.GetAllRoutesAsync())
                .FirstOrDefault(r => r.RouteNumber == uniqueRouteNumber);

            Assert.NotNull(createdRoute);

            // Modify the route
            createdRoute.ValidityPeriod = "2026-2027";
            createdRoute.DayValidity = "Weekdays";

            // Act
            var result = await _routeRepository.UpdateRouteAsync(createdRoute);

            // Assert
            Assert.True(result);

            // Verify the route was updated
            var updatedRoute = (await _routeRepository.GetRouteByIdAsync(createdRoute.RouteId));
            Assert.NotNull(updatedRoute);
            Assert.Equal("2026-2027", updatedRoute.ValidityPeriod);
            Assert.Equal("Weekdays", updatedRoute.DayValidity);
        }
        finally
        {
            // Clean up
            var createdRoute = (await _routeRepository.GetAllRoutesAsync())
                .FirstOrDefault(r => r.RouteNumber == uniqueRouteNumber);

            if (createdRoute != null)
            {
                await _routeRepository.DeleteRouteAsync(createdRoute.RouteId);
            }
        }
    }

    [Fact]
    public async Task DeleteRouteAsync_ShouldRemoveRouteFromDatabase()
    {
        // Arrange
        var uniqueRouteNumber = "TestRouteDelete_" + System.Guid.NewGuid().ToString();
        var newRoute = new Route
        {
            RouteNumber = uniqueRouteNumber,
            ValidityPeriod = "2025-2026",
            DayValidity = "Weekends",
            CompanyId = 1
        };

        // Create the route to delete
        await _routeRepository.CreateRouteAsync(newRoute);
        var createdRoute = (await _routeRepository.GetAllRoutesAsync())
            .FirstOrDefault(r => r.RouteNumber == uniqueRouteNumber);

        Assert.NotNull(createdRoute);

        // Act
        var result = await _routeRepository.DeleteRouteAsync(createdRoute.RouteId);

        // Assert
        Assert.True(result);

        // Verify the route was deleted
        var deletedRoute = await _routeRepository.GetRouteByIdAsync(createdRoute.RouteId);
        Assert.Null(deletedRoute);
    }
}

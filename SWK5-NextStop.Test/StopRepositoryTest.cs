using Xunit;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Threading.Tasks;
using System.Linq;

public class StopRepositoryIntegrationTest
{
    // Connection string for testing
    private readonly string _testConnectionString = "Host=localhost;Port=5432;Database=next_stop;Username=postgres;Password=";
    private readonly StopRepository _stopRepository;

    public StopRepositoryIntegrationTest()
    {
        var connectionFactory = new ConnectionFactory(_testConnectionString);
        _stopRepository = new StopRepository(connectionFactory);
    }

    [Fact]
    public async Task GetAllStopsAsync_ShouldReturnStopsFromDatabase()
    {
        // Act
        var result = await _stopRepository.GetAllStopsAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any()); // Ensure there is data in the test database
    }

    [Fact]
    public async Task CreateStopAsync_ShouldAddStopToDatabase()
    {
        // Arrange
        var uniqueStopName = "TestStop_" + System.Guid.NewGuid().ToString();
        var newStop = new Stop
        {
            Name = uniqueStopName,
            ShortName = "TS_" + System.Guid.NewGuid().ToString().Substring(0, 5),
            GpsCoordinates = "48.3358, 14.3173"
        };

        try
        {
            // Act
            var result = await _stopRepository.CreateStopAsync(newStop);

            // Assert
            Assert.True(result);

            // Verify the stop was added
            var createdStop = (await _stopRepository.GetAllStopsAsync())
                .FirstOrDefault(s => s.Name == uniqueStopName);

            Assert.NotNull(createdStop);
            Assert.Equal(uniqueStopName, createdStop.Name);
        }
        finally
        {
            // Clean up to maintain database state
            var createdStop = (await _stopRepository.GetAllStopsAsync())
                .FirstOrDefault(s => s.Name == uniqueStopName);

            if (createdStop != null)
            {
                await _stopRepository.DeleteStopAsync(createdStop.StopId);
            }
        }
    }

    [Fact]
    public async Task UpdateStopAsync_ShouldUpdateStopInDatabase()
    {
        // Arrange
        var uniqueStopName = "TestStopUpdate_" + System.Guid.NewGuid().ToString();
        var newStop = new Stop
        {
            Name = uniqueStopName,
            ShortName = "TSU_" + System.Guid.NewGuid().ToString().Substring(0, 5),
            GpsCoordinates = "48.3358, 14.3173"
        };

        try
        {
            // Create a new stop to update
            await _stopRepository.CreateStopAsync(newStop);
            var createdStop = (await _stopRepository.GetAllStopsAsync())
                .FirstOrDefault(s => s.Name == uniqueStopName);

            Assert.NotNull(createdStop);

            // Modify the stop
            createdStop.ShortName = "UpdatedSN";
            createdStop.GpsCoordinates = "48.3360, 14.3200";

            // Act
            var result = await _stopRepository.UpdateStopAsync(createdStop);

            // Assert
            Assert.True(result);

            // Verify the stop was updated
            var updatedStop = (await _stopRepository.GetStopByIdAsync(createdStop.StopId));
            Assert.NotNull(updatedStop);
            Assert.Equal("UpdatedSN", updatedStop.ShortName);
            Assert.Equal("48.3360, 14.3200", updatedStop.GpsCoordinates);
        }
        finally
        {
            // Clean up
            var createdStop = (await _stopRepository.GetAllStopsAsync())
                .FirstOrDefault(s => s.Name == uniqueStopName);

            if (createdStop != null)
            {
                await _stopRepository.DeleteStopAsync(createdStop.StopId);
            }
        }
    }

    [Fact]
    public async Task DeleteStopAsync_ShouldRemoveStopFromDatabase()
    {
        // Arrange
        var uniqueStopName = "TestStopDelete_" + System.Guid.NewGuid().ToString();
        var newStop = new Stop
        {
            Name = uniqueStopName,
            ShortName = "TSD_" + System.Guid.NewGuid().ToString().Substring(0, 5),
            GpsCoordinates = "48.3358, 14.3173"
        };

        // Create the stop to delete
        await _stopRepository.CreateStopAsync(newStop);
        var createdStop = (await _stopRepository.GetAllStopsAsync())
            .FirstOrDefault(s => s.Name == uniqueStopName);

        Assert.NotNull(createdStop);

        // Act
        var result = await _stopRepository.DeleteStopAsync(createdStop.StopId);

        // Assert
        Assert.True(result);

        // Verify the stop was deleted
        var deletedStop = await _stopRepository.GetStopByIdAsync(createdStop.StopId);
        Assert.Null(deletedStop);
    }
}

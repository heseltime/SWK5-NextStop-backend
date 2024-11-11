using Xunit;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;
using NextStop.Data;
using System.Threading.Tasks;
using System.Linq;
using System;

public class HolidayRepositoryIntegrationTest
{
    // Connection string for testing
    private readonly string _testConnectionString = "Host=localhost;Port=5432;Database=next_stop;Username=postgres;Password=";
    private readonly HolidayRepository _holidayRepository;

    public HolidayRepositoryIntegrationTest()
    {
        var connectionFactory = new ConnectionFactory(_testConnectionString);
        _holidayRepository = new HolidayRepository(connectionFactory);
    }

    [Fact]
    public async Task GetAllHolidaysAsync_ShouldReturnHolidaysFromDatabase()
    {
        // Act
        var result = await _holidayRepository.GetAllHolidaysAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Any()); // Ensure there is data in the test database
    }

    [Fact]
    public async Task CreateHolidayAsync_ShouldAddHolidayToDatabase()
    {
        // Arrange
        var newHoliday = new Holiday
        {
            Date = DateTime.Now.AddDays(30),
            Description = "Test Holiday",
            IsSchoolBreak = true,
            CompanyId = 0
        };

        try
        {
            // Act
            var result = await _holidayRepository.CreateHolidayAsync(newHoliday);

            // Assert
            Assert.True(result);

            // Verify the holiday was added
            var createdHolidays = (await _holidayRepository.GetAllHolidaysAsync());

            var createdHoliday = createdHolidays.FirstOrDefault(h => h.Description == "Test Holiday");

            Assert.NotNull(createdHoliday);
            Assert.Equal(newHoliday.Description, createdHoliday.Description);
        }
        finally
        {
            // Clean up to maintain database state
            var createdHoliday = (await _holidayRepository.GetAllHolidaysAsync())
                .FirstOrDefault(h => h.Description == "Test Holiday" && h.Date == newHoliday.Date);

            if (createdHoliday != null)
            {
                await _holidayRepository.DeleteHolidayAsync(createdHoliday.Id);
            }
        }
    }

    [Fact]
    public async Task UpdateHolidayAsync_ShouldUpdateHolidayInDatabase()
    {
        // Arrange
        var newHoliday = new Holiday
        {
            Date = DateTime.Now.AddDays(30),
            Description = "Test Holiday Update",
            IsSchoolBreak = false
        };

        try
        {
            // Create a new holiday to update
            await _holidayRepository.CreateHolidayAsync(newHoliday);
            var createdHoliday = (await _holidayRepository.GetAllHolidaysAsync())
                .FirstOrDefault(h => h.Description == "Test Holiday Update");

            Assert.NotNull(createdHoliday);

            // Modify the holiday
            createdHoliday.Description = "Updated Holiday";
            createdHoliday.IsSchoolBreak = true;

            // Act
            var result = await _holidayRepository.UpdateHolidayAsync(createdHoliday);

            // Assert
            Assert.True(result);

            // Verify the holiday was updated
            var updatedHoliday = await _holidayRepository.GetHolidayByIdAsync(createdHoliday.Id);
            Assert.NotNull(updatedHoliday);
            Assert.Equal("Updated Holiday", updatedHoliday.Description);
            Assert.True(updatedHoliday.IsSchoolBreak);
        }
        finally
        {
            // Clean up
            var createdHoliday = (await _holidayRepository.GetAllHolidaysAsync())
                .FirstOrDefault(h => h.Description == "Updated Holiday" || h.Description == "Test Holiday Update");

            if (createdHoliday != null)
            {
                await _holidayRepository.DeleteHolidayAsync(createdHoliday.Id);
            }
        }
    }

    [Fact]
    public async Task DeleteHolidayAsync_ShouldRemoveHolidayFromDatabase()
    {
        // Arrange
        var newHoliday = new Holiday
        {
            Date = DateTime.Now.AddDays(30),
            Description = "Test Holiday Delete",
            IsSchoolBreak = false
        };

        // Create the holiday to delete
        await _holidayRepository.CreateHolidayAsync(newHoliday);
        var createdHoliday = (await _holidayRepository.GetAllHolidaysAsync())
            .FirstOrDefault(h => h.Description == "Test Holiday Delete");

        Assert.NotNull(createdHoliday);

        // Act
        var result = await _holidayRepository.DeleteHolidayAsync(createdHoliday.Id);

        // Assert
        Assert.True(result);

        // Verify the holiday was deleted
        var deletedHoliday = await _holidayRepository.GetHolidayByIdAsync(createdHoliday.Id);
        Assert.Null(deletedHoliday);
    }
}

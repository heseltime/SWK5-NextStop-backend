using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SWK5_NextStop;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
public class HolidayControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HolidayControllerIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllHolidays_ShouldReturnOkWithHolidays()
    {
        // Act
        var response = await _client.GetAsync("/api/holiday");

        // Assert
        response.EnsureSuccessStatusCode(); // Status code 200-299
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
        Assert.Contains("Holiday", responseString); // Simplified check for content
    }
}
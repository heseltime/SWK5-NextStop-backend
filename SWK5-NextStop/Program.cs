using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Enables controller support
builder.Services.AddEndpointsApiExplorer(); // For minimal API endpoint documentation
builder.Services.AddSwaggerGen(); // Enables Swagger for API documentation

// Register repositories with dependency injection
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new ConnectionFactory(connectionString);
});

builder.Services.AddSingleton<RouteRepository>();
builder.Services.AddSingleton<HolidayRepository>();
builder.Services.AddSingleton<StopRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger in development
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization(); // Placeholder for future authentication/authorization

// Map controller routes
app.MapControllers();

// Uncomment to add minimal API support (optional, if used alongside controllers)
// app.MapGet("/weatherforecast", () => 
//     Results.Ok(new List<WeatherForecast> 
//     { 
//         new WeatherForecast(DateOnly.FromDateTime(DateTime.Now), 25, "Sunny") 
//     }));

app.Run();

// Record for minimal API example (optional, for testing purposes)
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
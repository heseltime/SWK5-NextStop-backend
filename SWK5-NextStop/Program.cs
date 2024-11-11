using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Npgsql;

using NextStop.Data;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sample API to test 
app.MapGet("/holidays", async (HolidayRepository repository) =>
{
    var holidays = await repository.GetAllHolidaysAsync();
    return Results.Ok(holidays);
}).WithName("GetAllHolidays").WithOpenApi();

app.MapGet("/stops", async (StopRepository repository) =>
{
    var stops = await repository.GetAllStopsAsync();
    return Results.Ok(stops);
}).WithName("GetAllStops").WithOpenApi();

app.MapGet("/routes", async (RouteRepository repository) =>
{
    var routes = await repository.GetAllRoutesAsync();
    return Results.Ok(routes);
}).WithName("GetAllRoutes").WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
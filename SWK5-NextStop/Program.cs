using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;
using SWK5_NextStop.Service;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(options =>
{
    // Ensure all errors, including model binding errors, are detailed
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
})
.ConfigureApiBehaviorOptions(options =>
{
    // Customize validation error responses
    options.InvalidModelStateResponseFactory = context =>
    {
        var problemDetails = new
        {
            Title = "Invalid Request",
            Status = 400,
            Errors = context.ModelState
                .Where(m => m.Value.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
        };

        return new BadRequestObjectResult(problemDetails);
    };
}); // Enables controller support
builder.Services.AddEndpointsApiExplorer(); // For minimal API endpoint documentation
builder.Services.AddSwaggerGen(); // Enables Swagger for API documentation

// Configure authentication with OAuth and OpenID Connect
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var configuration = builder.Configuration;
        options.Authority = "https://dev-jkdeqiuo4mmcn3r8.us.auth0.com/";
        options.Audience = "https://next-stop-khg/";
        options.RequireHttpsMetadata = false; // Allow HTTP connections
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

// Register repositories (with dependency injection) and other singletons
builder.Services.AddSingleton<IConnectionFactory>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new ConnectionFactory(connectionString);
});
// -- Repositories themselves
builder.Services.AddSingleton<RouteRepository>();
builder.Services.AddSingleton<HolidayRepository>();
builder.Services.AddSingleton<StopRepository>();
builder.Services.AddSingleton<ScheduleRepository>();

// -- Services
builder.Services.AddSingleton<HolidayService>();
builder.Services.AddSingleton<RouteService>();
builder.Services.AddSingleton<StopService>();
builder.Services.AddSingleton<ScheduleService>();
builder.Services.AddSingleton<IApiKeyValidator, ApiKeyValidator>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allow only Angular app
            .AllowAnyHeader() // Allow all headers
            .AllowAnyMethod(); // Allow all HTTP methods (GET, POST, PUT, DELETE, etc.)
    });
});

// other
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
    });

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger in development
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp"); // Apply the CORS policy
app.UseRouting();

// Add middleware for detailed error logging
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var problemDetails = new
        {
            Title = "An unexpected error occurred",
            Status = 500,
            Detail = ex.Message
        };

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }
});

// Add authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization(); 

// Map controller routes
app.MapControllers();

app.Run();

public partial class Program { } // entry point to start the application under test

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SWK5_NextStop.DAL;
using SWK5_NextStop.Infrastructure;
using SWK5_NextStop.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers(); // Enables controller support
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

// -- Services
builder.Services.AddSingleton<HolidayService>();
builder.Services.AddSingleton<RouteService>();
builder.Services.AddSingleton<StopService>();

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger in development
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Add authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization(); 

// Map controller routes
app.MapControllers();

app.Run();

public partial class Program { } // entry point to start the application under test

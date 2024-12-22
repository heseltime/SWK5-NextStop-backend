namespace SWK5_NextStop.Service;

public class ApiKeyValidator : IApiKeyValidator
{
    private readonly IConfiguration _configuration;

    public ApiKeyValidator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool ValidateApiKey(string apiKey)
    {
        // Retrieve valid keys from configuration or database
        var validApiKeys = _configuration.GetSection("ApiKeys").Get<List<string>>() ?? new List<string>();

        return validApiKeys.Contains(apiKey);
    }
}

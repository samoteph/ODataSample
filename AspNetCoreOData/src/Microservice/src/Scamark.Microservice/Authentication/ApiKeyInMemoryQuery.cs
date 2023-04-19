using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Scamark.Microservice.Authentication;

public class ApiKeyInMemoryQuery : IApiKeyQuery
{
    private ConcurrentDictionary<string, ApiKey> _apiKeys;
    private readonly ILogger<ApiKeyInMemoryQuery> _logger;

    public ApiKeyInMemoryQuery(IOptionsMonitor<AuthenticationOptions> optionsMonitorConfig, ILogger<ApiKeyInMemoryQuery> logger)
    {
        if (optionsMonitorConfig is null)
        {
            throw new ArgumentNullException(nameof(optionsMonitorConfig));
        }
        UpdateConfig(optionsMonitorConfig.CurrentValue);
        optionsMonitorConfig.OnChange(UpdateConfig);
        _logger = logger;
    }

    public ApiKey Execute(string providedApiKey)
    {
        if (_apiKeys == null)
        {
            return null;
        }
        if (_apiKeys.TryGetValue(providedApiKey, out var key) == false)
        {
            return null;
        }
        if (key.StartDate > DateTime.Now)
        {
            return null;
        }
        if (key.EndDate != null && key.EndDate < DateTime.Now)
        {
            return null;
        }
        return key;
    }

    private void UpdateConfig(AuthenticationOptions config)
    {
        var apiKeys = config.ApiKeys;

        if (apiKeys is null || apiKeys.Any() == false)
        {
            _apiKeys = new ConcurrentDictionary<string, ApiKey>();
            _logger?.LogWarning("Aucune clé d'API n'a été ajoutée dans la configuration.");
            return;
        }

        _apiKeys = new ConcurrentDictionary<string, ApiKey>(apiKeys.ToDictionary(x => x.ClientSecret, x => x));
        var apps = string.Join(",", apiKeys.Select(x => x.ClientId));
        _logger?.LogInformation("Les clés d'API sont configurées pour {ClientCount} applications : {Clients} ", _apiKeys.Count, apps);
    }
}

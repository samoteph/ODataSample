using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Framework.Common.Logging;
using Scamark.Microservice.ApplicationInsights;

namespace Scamark.Microservice;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddApplicationInsights(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITelemetryInitializer, CloudManifestTelemetryInitializer>();
        services.AddSingleton<ITelemetryInitializer, IgnoreHttpNotFoundTelemetryInitializer>();
        services.AddApplicationInsightsTelemetryProcessor<FilterHealthCheckssTelemetryProcessor>();

        // cf https://docs.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core
        services.AddApplicationInsightsTelemetry(configuration);
        services.AddApplicationInsightsKubernetesEnricher();

        return services;
    }
}

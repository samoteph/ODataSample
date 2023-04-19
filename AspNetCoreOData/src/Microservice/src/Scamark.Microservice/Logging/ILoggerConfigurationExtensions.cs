using Serilog;
using Serilog.Filters;

namespace Scamark.Microservice.Logging;

public static class LoggerConfigurationExtensions
{
    public static LoggerConfiguration ExcludeHealthChecks(this LoggerConfiguration loggerConfiguration)
    {
        if (loggerConfiguration is null)
        {
            throw new ArgumentNullException(nameof(loggerConfiguration));
        }
        // filtre tous les logs envoyés sur /health
        loggerConfiguration.Filter.ByExcluding(Matching.WithProperty("RequestPath", (string s) => s.EndsWith(Constants.HealthCheckPath)));
        return loggerConfiguration;
    }
}

using Microsoft.Extensions.Hosting;

namespace Scamark.Microservice;

public static class IHostEnvironmentExtensions
{
    public static bool IsLocal(this IHostEnvironment environment)
        => string.Equals(environment?.EnvironmentName, "Local", StringComparison.OrdinalIgnoreCase);
}

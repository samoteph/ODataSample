using System.Runtime.CompilerServices;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Scamark.Microservice.ApplicationInsights;

public class FilterHealthCheckssTelemetryProcessor : ITelemetryProcessor
{
    private const string HealthCheckOperationPath = $"GET {Constants.HealthCheckPath}";

    public FilterHealthCheckssTelemetryProcessor(ITelemetryProcessor next)
    {
        Next = next;
    }

    private ITelemetryProcessor Next { get; }

    public void Process(ITelemetry item)
    {
        // To filter out an item, return without calling the next processor.
        if (item != null && ShouldSendTelemetry(item) == false)
        {
            return;
        }

        Next.Process(item);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool ShouldSendTelemetry(ITelemetry item)
    {
        if (item is ExceptionTelemetry)
        {
            return true;
        };

        if (item.Context?.Operation?.Name?.Equals(HealthCheckOperationPath) == true)
        {
            return false;
        }

        return true;
    }
}

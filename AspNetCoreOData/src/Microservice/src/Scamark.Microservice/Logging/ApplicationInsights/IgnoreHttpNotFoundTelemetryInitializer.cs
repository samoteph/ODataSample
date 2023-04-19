using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Scamark.Microservice.ApplicationInsights;

/// <summary>
/// Cette classe permet de dire à AI de ne pas considérer les erreurs 404 comme des exceptions, puisque c'est un code de retour valide
/// pour une API lorsque l'entité n'est pas trouvée.
/// </summary>
public class IgnoreHttpNotFoundTelemetryInitializer : ITelemetryInitializer
{
    private static readonly string Status404NotFoundString = StatusCodes.Status404NotFound.ToString();

    public void Initialize(ITelemetry telemetry)
    {
        var requestTelemetry = telemetry as RequestTelemetry;

        if (requestTelemetry == null)
        {
            return;
        }

        if (requestTelemetry.ResponseCode == Status404NotFoundString && requestTelemetry.Name?.StartsWith("GET /") == true)
        {
            // If we set the Success property, the SDK won't change it:
            requestTelemetry.Success = true;
        }
    }
}

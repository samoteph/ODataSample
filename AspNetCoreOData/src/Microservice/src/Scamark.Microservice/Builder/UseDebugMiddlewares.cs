using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Scamark.Microservice;

public static partial class AppBuilderExtensions
{
    public static void UseDebugMiddlewares(this IApplicationBuilder app, IConfiguration config, ILogger logger)
    {
        if (string.Equals(config.GetValue<string>("Scamark:Debug:UseMiddlewares"), "true", StringComparison.OrdinalIgnoreCase) == false)
        {
            return;
        }

        logger.LogInformation("Scamark:Debug:UseMiddlewares is true, using debug middlewares.");
        app.UseMiddleware<DebugMiddleware>();
    }
}

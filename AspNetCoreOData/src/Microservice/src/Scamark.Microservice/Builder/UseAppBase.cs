using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Scamark.Microservice;

public static partial class AppBuilderExtensions
{
    public static void UseAppBase(this IApplicationBuilder app, IConfiguration config, ILogger logger)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (config is null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        // gère le cas de l'API derrière un reverse proxy (ie: Traefik)        
        var routePrefix = config["Scamark:RoutePrefix"];

        if (string.IsNullOrWhiteSpace(routePrefix) == false)
        {
            var pathBase = $"/{routePrefix}/";
            logger.LogInformation("Using PathBase from Scamark:RoutePrefix {RoutePrefix}", pathBase);
            app.UsePathBase(pathBase);
        }
    }
}

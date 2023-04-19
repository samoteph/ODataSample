using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData;
using Scamark.Framework.Common;
using Serilog;

namespace Scamark.Microservice;

public static partial class AppBuilderExtensions
{
    /// <summary>
    /// Ajoute les middlewares nécessaires à l'exposition d'une endpoint OData sur la route /odata/
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseScamarkOData(this WebApplication app, bool useVersioning = true)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        var startupType = typeof(AppBuilderExtensions);
        var config = app.Services.GetRequiredService<IConfiguration>();
        var env = app.Services.GetRequiredService<IWebHostEnvironment>();
        var logger = (Microsoft.Extensions.Logging.ILogger)app.Services.GetRequiredService(typeof(ILogger<>).MakeGenericType(startupType));
        var defaultVersion = GetDefaultODataApiVersion(config);
        var doNotRewriteUrlWithSpaces = config.GetValue<bool?>("Scamark:API:DoNotRewriteUrlWithSpaces");

        if (config.GetValue<bool?>("FORWARDEDHEADERS_ENABLED") == true)
        {
            app.UseForwardedHeaders();
        }

        app.UseAppBase(config, logger);

        var useDeveloperExceptionPage = config.GetValue<bool?>("Scamark:API:UseDeveloperExceptionPage");

        if (((env.IsDevelopment() || env.IsLocal()) && useDeveloperExceptionPage != false) || useDeveloperExceptionPage == true)
        {
            app.UseDeveloperExceptionPage();

            // navigate to ~/$odata to determine whether any endpoints did not match an odata route template
            app.UseODataRouteDebug();
        }
        else
        {
            //app.UseStatusCodePages(async statusCodeContext =>
            //{
            //    statusCodeContext.HttpContext.Response.ContentType = Text.Plain;
            //    await statusCodeContext.HttpContext.Response.WriteAsync(
            //        $"StatusCode: {statusCodeContext.HttpContext.Response.StatusCode}, TraceIdentifier : {statusCodeContext.HttpContext.TraceIdentifier}");
            //});
        }

        app.UseSerilogRequestLogging();
        // à déclarer avant odata car le composant catch ensuite toutes les requêtes sur /
        app.UseDebugMiddlewares(config, logger);
        app.UseSwagger(defaultVersion, useVersioning);

        // fix bug when special characters slash and space are present in the url odata parameters
        if (doNotRewriteUrlWithSpaces != false)
        {
            UseRewriteSpaceMiddleware(app);
        }

        // call custom startup hooks
        var hostStartups = app.Services.GetServices<HostStartup>();
        foreach (var evt in hostStartups.Select(x => x.StartingEvent))
        {
            evt.Invoke(app, logger, config);
        }

        // à déclarer en dernier
        UseOData(app, config);

        return app;
    }

    private static void UseRewriteSpaceMiddleware(IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.Value.Contains(' '))
            {
                var path = context.Request.Path.Value.Replace(" ", "%20");
                context.Request.Path = new PathString(path);
            }

            await next(context);
        });
    }

    private static void UseOData(WebApplication app, IConfiguration config)
    {
        var batchModeEnabled = config.GetValue<bool?>("Scamark:API:OData:BatchModeEnabled") == true;
        var appManifest = app.Services.GetRequiredService<ApplicationManifest>();
        var apiName = appManifest.Name;

        if (batchModeEnabled == true)
        {
            // à appeler avant UseRouting - cf. https://devblogs.microsoft.com/odata/asp-net-odata-8-0-preview-for-net-5/
            // https://github.com/microsoft/aspnet-api-versioning/wiki/OData-Batching
            app.UseODataBatching();
        }

        // Add OData /$query middleware
        // Replace UseODataQueryRequest() by UseCustomODataQueryRequest() to support OData function
        app.UseCustomODataQueryRequest();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapHealthChecks(Constants.HealthCheckPath).AllowAnonymous();
            endpoints.MapControllers();
        });
    }

    private static int GetDefaultODataApiVersion(IConfiguration config)
    {
        return config.GetValue<int?>("Scamark:API:OData:DefaultVersion") ?? 1;
    }
}

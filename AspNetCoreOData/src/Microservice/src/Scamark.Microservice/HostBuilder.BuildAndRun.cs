using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Scamark.Framework.Common;

namespace Scamark.Microservice;

public static class HostBuilderExtensions
{
    public static int BuildAndRun(this IWebHostBuilder hostBuilder)
    {
        if (hostBuilder is null)
        {
            throw new ArgumentNullException(nameof(hostBuilder));
        }

        IWebHost host;
        try
        {
            host = hostBuilder.Build();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "WebHost terminated unexpectedly while building.");
            return -1;
        }

        var appManifest = host.Services.GetRequiredService<ApplicationManifest>();
        var config = host.Services.GetRequiredService<IConfiguration>();
        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("WebHost");

        logger.LogInformation("Scamark Host starting - {@AppInfo}", appManifest);

        try
        {
            host.Run();
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "WebHost terminated unexpectedly while running.");
            return -1;
        }
        finally
        {
            host?.Dispose();
        }

        Serilog.Log.Debug("WebHost stopped.");

        return 0;
    }
}

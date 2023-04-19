using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Framework.Common;
using Scamark.Framework.Common.Logging;
using Scamark.Microservice.Logging;
using Scamark.Microservice.OData.Builder;
using Serilog;

namespace Scamark.Microservice;

public static class ScamarkHostBuilder
{
    /// <summary>
    /// Créé la configuration par défaut d'un hôte ASP.NET Core de microservice Scamark avec OData.
    /// </summary>
    /// <typeparam name="TStartup">Classe contenant la configuration des services.</typeparam>
    /// <param name="args">Arguments de ligne de commande.</param>
    /// <param name="useVersioning">Détermine si le composant versioning OData est utilisé.</param>
    /// <returns>Un builder d'hôte.</returns>
    public static WebApplicationBuilder CreateODataWebHost<TModelConfiguration>(
        string[] args, bool useVersioning = true) where TModelConfiguration : IVersionedModelConfiguration, new()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        var config = new ConfigurationBuilder().ScamarkDefault(args).Build();
        var appManifest = ApplicationManifest.CreateFromConfiguration(config, ApplicationType.API);

        // initialise le logger par défaut
        var loggerConfiguration = SerilogStaticLogger.GetLoggerConfiguration(config, appManifest);
        loggerConfiguration.ExcludeHealthChecks();

        SerilogStaticLogger.Init(config, loggerConfiguration);

        if (config.GetValue<bool?>("Scamark:API:HanaDisabled") != true)
        {
            Hana.AssemblyHelper.LoadFromBaseDirectory();
        }

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddConfiguration(config);
            var reloadOnChange = builder.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
            builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: reloadOnChange);
            builder.Configuration.AddJsonFile("appsettings.ApiKeys.json", optional: true, reloadOnChange: reloadOnChange);
            builder.Logging.AddSerilog();
            builder.Host.UseSerilog();
            var services = builder.Services;
            services.AddSingleton(appManifest);
            var mvcBuilder = ServiceCollectionExtensions.AddScamarkCommon(services, builder.Configuration);
            ServiceCollectionExtensions.AddOData<TModelConfiguration>(services, config, mvcBuilder, appManifest, useVersioning);
            return builder;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "WebHost terminated unexpectedly while configuring.");
            throw;
        }
    }

    /// <summary>
    /// Créé la configuration par défaut d'un hôte ASP.NET Core de microservice Scamark.
    /// </summary>
    /// <typeparam name="TStartup">Classe contenant la configuration des services.</typeparam>
    /// <param name="args">Arguments de ligne de commande.</param>
    /// <param name="useODataServer">Détermine si le microservice expose des données en OData.</param>
    /// <returns>Un builder d'hôte.</returns>
    public static WebApplicationBuilder CreateWebHost(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

        var config = new ConfigurationBuilder().ScamarkDefault(args).Build();
        var appManifest = ApplicationManifest.CreateFromConfiguration(config, ApplicationType.API);

        // initialise le logger par défaut
        var loggerConfiguration = SerilogStaticLogger.GetLoggerConfiguration(config, appManifest);
        loggerConfiguration.ExcludeHealthChecks();

        SerilogStaticLogger.Init(config, loggerConfiguration);

        if (config.GetValue<bool?>("Scamark:API:HanaDisabled") != true)
        {
            Hana.AssemblyHelper.LoadFromBaseDirectory();
        }

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddConfiguration(config);
            var reloadOnChange = builder.Configuration.GetValue("hostBuilder:reloadConfigOnChange", defaultValue: true);
            builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: reloadOnChange);
            builder.Configuration.AddJsonFile("appsettings.ApiKeys.json", optional: true, reloadOnChange: reloadOnChange);
            builder.Logging.AddSerilog();
            builder.Host.UseSerilog();
            var services = builder.Services;
            services.AddSingleton(appManifest);
            var mvcBuilder = ServiceCollectionExtensions.AddScamarkCommon(services, builder.Configuration);
            return builder;
        }
        catch (Exception ex)
        {
            Serilog.Log.Error(ex, "WebHost terminated unexpectedly while configuring.");
            throw;
        }
    }
}

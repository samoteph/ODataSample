using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Framework.Common;
using Scamark.Microservice.Authentication;
using Scamark.Microservice.Converters;
using Scamark.Microservice.Security;

namespace Scamark.Microservice;

public static partial class ServiceCollectionExtensions
{
    internal static IMvcBuilder AddScamarkCommon(IServiceCollection services, IConfiguration configuration)
    {
        if (configuration["FORWARDEDHEADERS_ENABLED"] == "true")
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });
        }

        AddSwagger(services);

        // register AI if used
        if (configuration.GetApplicationInsightsKey() != null)
        {
            AddApplicationInsights(services, configuration);
        }

        var mvcBuilder = AddControllers(services);

        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptions.ConfigSectionKey));
        services.AddScamarkAuthentication();

        AddHealthCheck(services);

        return mvcBuilder;
    }

    private static IMvcBuilder AddControllers(IServiceCollection services)
    {
        var mvcBuilder = services
            .AddControllers(mvcOptions =>
            {
                mvcOptions.EnableEndpointRouting = true;
                mvcOptions.Conventions.Add(new AuthorizationActionConvention());
            })
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
                jsonOptions.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
                // Commentaire Adrien :
                // -> cf https://github.com/dotnet/runtime/issues/782
                // -> cf https://github.com/OData/AspNetCoreOData/blob/main/docs/camel_case_scenarios.md
                // JsonNamingPolicy.PascalCase devrait être ajouté dans une version future de .NET
                ////jsonOptions.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.PascalCase;
            });

        ServiceCollectionExtensions.AddCustomBadRequest(services);

        return mvcBuilder;
    }

    internal static void AddHealthCheck(IServiceCollection services)
    {
        services.AddHealthChecks();
    }
}

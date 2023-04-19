using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Microservice.Authentication;
using Scamark.Microservice.Authorization;
using Scamark.Microservice.Security;

namespace Scamark.Microservice;

public static partial class ServiceCollectionExtensions
{
    public static void AddScamarkAuthentication(this IServiceCollection services)
    {
        const string apiAuthAuthenticateScheme = ApiKeyConstants.DefaultScheme;

        services
            .AddAuthentication(options =>
            {
            })
            .AddApiKeySupport(apiAuthAuthenticateScheme, options =>
            {
                options.Scheme = apiAuthAuthenticateScheme;
                options.AuthenticationType = apiAuthAuthenticateScheme;
            });

        services
            .AddAuthorization(options =>
            {
                options.AddPolicy(ApiPolicy.ScamarkMetadataRead.ToString(), policy => policy.RequireAuthenticatedUser().RequireRole(Roles.ApiScamarkMetadataRead));
                options.AddPolicy(ApiPolicy.ScamarkRead.ToString(), policy => policy.RequireAuthenticatedUser().RequireRole(Roles.ApiScamarkRead));
                options.AddPolicy(ApiPolicy.ScamarkWrite.ToString(), policy => policy.RequireAuthenticatedUser().RequireRole(Roles.ApiScamarkWrite));
                options.AddPolicy(ApiPolicy.SapRead.ToString(), policy => policy.RequireAuthenticatedUser().RequireRole(Roles.ApiSapRead));
                options.AddPolicy(ApiPolicy.SapWrite.ToString(), policy => policy.RequireAuthenticatedUser().RequireRole(Roles.ApiSapWrite));

                // todo Sébastien : il faudra ajouter le nom du Scheme de l'auth avec Identity Server
                var defaultAuth = new AuthorizationPolicyBuilder(apiAuthAuthenticateScheme).RequireAuthenticatedUser().Build();

                options.DefaultPolicy = defaultAuth;
                options.FallbackPolicy = defaultAuth;
            });
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Scamark.Microservice.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddApiKeySupport(
        this AuthenticationBuilder authenticationBuilder,
        string authenticationScheme,
        Action<ApiKeyAuthenticationOptions> options)
    {
        if (authenticationBuilder is null)
        {
            throw new ArgumentNullException(nameof(authenticationBuilder));
        }

        authenticationBuilder.Services.AddSingleton<IApiKeyQuery, ApiKeyInMemoryQuery>();
        return authenticationBuilder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(authenticationScheme, options);
    }
}

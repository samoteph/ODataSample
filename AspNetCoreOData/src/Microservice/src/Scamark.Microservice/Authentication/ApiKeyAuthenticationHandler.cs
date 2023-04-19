using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scamark.Microservice.Security;

namespace Scamark.Microservice.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string ProblemDetailsContentType = "application/problem+json";
    private readonly ILoggerFactory _loggerFactory;
    private readonly IApiKeyQuery _getApiKeyQuery;
    private readonly bool _bypassAuth = false;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        IOptions<AuthenticationOptions> scamarkAuthOptions,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyQuery getApiKeyQuery) : base(options, loggerFactory, encoder, clock)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (scamarkAuthOptions is null)
        {
            throw new ArgumentNullException(nameof(scamarkAuthOptions));
        }

        if (encoder is null)
        {
            throw new ArgumentNullException(nameof(encoder));
        }

        if (clock is null)
        {
            throw new ArgumentNullException(nameof(clock));
        }

        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _getApiKeyQuery = getApiKeyQuery ?? throw new ArgumentNullException(nameof(getApiKeyQuery));
        _bypassAuth = scamarkAuthOptions.Value.Disabled;
        Logger = new Lazy<ILogger>(() => _loggerFactory.CreateLogger<ApiKeyAuthenticationHandler>());
    }

    private new Lazy<ILogger> Logger { get; }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return Task.FromResult(HandleAuthenticate());
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        Response.ContentType = ProblemDetailsContentType;
        var problemDetails = new UnauthorizedProblemDetails();
        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonDefaultSerializerOptions.Options));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = (int)HttpStatusCode.Forbidden;
        Response.ContentType = ProblemDetailsContentType;
        var problemDetails = new ForbiddenProblemDetails();
        await Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonDefaultSerializerOptions.Options));
    }

    private AuthenticateResult HandleAuthenticate()
    {
        if (_bypassAuth == true)
        {
            Logger.Value?.LogTrace("Authentication bypassed - DEV MODE");
            return AuthenticateResult.Success(CreateMockedPrincipal());
        }

        if (Request.Headers.TryGetValue(ApiKeyConstants.HeaderName, out var apiKeyHeaderValues) == false)
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyHeaderValues.Count == 0 || string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var existingApiKey = _getApiKeyQuery.Execute(providedApiKey);

        if (existingApiKey == null)
        {
            Logger.Value?.LogTrace("Invalid API Key provided: {ApiKey}", providedApiKey);
            return AuthenticateResult.Fail("Invalid API Key provided.");
        }

        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingApiKey.ClientId)
            };

        claims.AddRange(existingApiKey.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var ticket = CreateTicketFromClaims(claims);

        return AuthenticateResult.Success(ticket);
    }

    private AuthenticationTicket CreateMockedPrincipal()
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "ScamarkDevUser"),
            };

        claims.AddRange(Roles.AllRoles.Select(role => new Claim(ClaimTypes.Role, role)));
        return CreateTicketFromClaims(claims);
    }

    private AuthenticationTicket CreateTicketFromClaims(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, Options.Scheme);
        return ticket;
    }
}

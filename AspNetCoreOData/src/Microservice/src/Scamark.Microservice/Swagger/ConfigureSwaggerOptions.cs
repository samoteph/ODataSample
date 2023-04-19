using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Scamark.Framework.Common;
using Scamark.Microservice.Authentication;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scamark.Microservice;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _apiVersions;
    private readonly int _defaultVersion;
    private readonly OpenApiConfiguration _openApiConfig;

    public ConfigureSwaggerOptions(IConfiguration configuration, ApplicationManifest appManifest, IServiceProvider serviceProvider)
    {
        if (configuration is null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }
        if (appManifest is null)
        {
            throw new ArgumentNullException(nameof(appManifest));
        }

        _apiVersions = serviceProvider.GetService<IApiVersionDescriptionProvider>();
        _defaultVersion = configuration.GetValue<int?>("Scamark:API:OData:DefaultVersion") ?? 1;
        _openApiConfig = new OpenApiConfiguration(
            name: appManifest.Name,
            title: configuration.GetValue<string>("Scamark:API:Title") ?? appManifest.DisplayName,
            description: configuration.GetValue<string>("Scamark:API:Description"),
            license: configuration.GetValue<string>("Scamark:API:License"),
            contactName: configuration.GetValue<string>("Scamark:API:ContactName"),
            contactEmail: configuration.GetValue<string>("Scamark:API:ContactEmail"),
            contactUrl: configuration.GetValue<Uri>("Scamark:API:ContactUrl"),
            appVersion: configuration.GetValue<string>("Scamark:Version") ?? appManifest.Version?.ToString()
        );
    }

    public void Configure(SwaggerGenOptions options)
    {
        if (_apiVersions != null)
        {
            foreach (var description in _apiVersions.ApiVersionDescriptions)
            {
                ConfigureSwaggerDoc(options, description.ApiVersion, description.GroupName, description.IsDeprecated);
            }
        }
        else
        {
            ConfigureSwaggerDoc(options, new ApiVersion(_defaultVersion, 0), $"v{_defaultVersion}", false);
        }
    }

    private void ConfigureSwaggerDoc(SwaggerGenOptions options, ApiVersion apiVersion, string groupName, bool isDeprecated)
    {
        var openApiInfo = new OpenApiInfo()
        {
            Title = _openApiConfig.Title,
            License = new OpenApiLicense
            {
                Name = _openApiConfig.License
            },
            Description = _openApiConfig.Description,
            Version = apiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = _openApiConfig.ContactName,
                Email = _openApiConfig.ContactEmail,
                Url = _openApiConfig.ContactUrl,
            }
        };

        if (isDeprecated == true)
        {
            openApiInfo.Description += " This API version has been deprecated.";
        }

        openApiInfo.Extensions.Add("x-scamark-app-version", new OpenApiString(_openApiConfig.AppVersion));

        options.SwaggerDoc(groupName, openApiInfo);

        options.AddSecurityDefinition(ApiKeyConstants.HeaderName, new OpenApiSecurityScheme
        {
            Description = "Clé d'API à utiliser pour accéder aux ressources.",
            In = ParameterLocation.Header,
            Name = ApiKeyConstants.HeaderName,
            Type = SecuritySchemeType.ApiKey
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Name = ApiKeyConstants.HeaderName,
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeyConstants.HeaderName },
                },
                new string[] {}
            }
        });
    }
}

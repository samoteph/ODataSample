using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Framework.Common;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Scamark.Microservice;

public static partial class AppBuilderExtensions
{
    public static void UseSwagger(this IApplicationBuilder app, int defaultVersion, bool useVersioning = true)
    {
        if (app is null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        var appManifest = app.ApplicationServices.GetRequiredService<ApplicationManifest>();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var apiName = appManifest.Name;
            options.DocumentTitle = (string.IsNullOrEmpty(apiName) == false ? ($"{apiName} - ") : string.Empty) + "Scamark - Swagger";

            if (useVersioning == true)
            {
                AddVersionedSwaggerEndpoint(app, options, apiName);
            }
            else
            {
                options.SwaggerEndpoint($"v{defaultVersion}/swagger.json", $"API {apiName}");
            }

            // ne pas afficher les Schemas, beaucoup de classes systèmes s'y retrouvent à tort (classes OData)
            options.DefaultModelsExpandDepth(-1);
        });
    }

    private static void AddVersionedSwaggerEndpoint(IApplicationBuilder app, SwaggerUIOptions options, string apiName)
    {
        // build a swagger endpoint for each discovered API version
        var apiVersionProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();
        //var description = app.DescribeApiVersions();
        foreach (var description in apiVersionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"{description.GroupName}/swagger.json", $"API {apiName} {description.GroupName.ToUpperInvariant()}");
        }
    }
}

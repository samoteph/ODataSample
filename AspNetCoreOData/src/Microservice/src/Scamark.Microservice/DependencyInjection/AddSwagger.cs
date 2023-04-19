using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scamark.Microservice.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scamark.Microservice;

public static partial class ServiceCollectionExtensions
{
    public static void AddSwagger(IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSwaggerGen(
            options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerDefaultValues>();

                // removes Void Schema : cela fait des erreurs
                // options.SchemaFilter<RemoveVoidSchemaFilter>();

                // include Xml comments of all Scamark assemblies
                IncludeAllXmlComments(options);

                // corrige le problème de méthodes ayant la même signature avec les noms courts
                var schemaHelper = new SwashbuckleSchemaHelper();
                options.CustomSchemaIds(type => schemaHelper.GetSchemaId(type));
            });
    }

    private static void IncludeAllXmlComments(SwaggerGenOptions options)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var xmlDocPath = Path.GetDirectoryName(entryAssembly.Location);

        foreach (var referencedAssembly in entryAssembly.GetReferencedAssemblies().Where(x => x.Name.StartsWith("Scamark.")))
        {
            IncludeXmlComments(options, referencedAssembly, xmlDocPath, false);
        }

        IncludeXmlComments(options, entryAssembly.GetName(), xmlDocPath, true);
    }

    private static void IncludeXmlComments(SwaggerGenOptions options, AssemblyName assembly, string xmlDocPath, bool warnIfNotFound = false)
    {
        var xmlDocumentationPath = Path.Combine(xmlDocPath, $"{assembly.Name}.xml");

        if (File.Exists(xmlDocumentationPath) == true)
        {
            options.IncludeXmlComments(xmlDocumentationPath);
        }
        else if (warnIfNotFound == true)
        {
            Serilog.Log.Warning($"Documentation XML non trouvée pour l'assembly {assembly.Name}.");
        }
    }
}

using Asp.Versioning;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OData.ModelBuilder;
using Scamark.Framework.Common;
using Scamark.Microservice.OData;
using Scamark.Microservice.OData.Builder;

namespace Scamark.Microservice;

public static partial class ServiceCollectionExtensions
{
    internal static void AddOData<TModelConfiguration>(
        IServiceCollection services,
        IConfiguration config,
        IMvcBuilder mvcBuilder,
        ApplicationManifest appManifest,
        bool useVersioning = true)
        where TModelConfiguration : IVersionedModelConfiguration, new()
    {
        // Add json odata query parser
        services.TryAddEnumerable(
            ServiceDescriptor.Singleton<IODataQueryRequestParser, JsonODataQueryRequestParser>());

        var maxTop = config.GetValue<int?>("Scamark:API:OData:MaxTop") ?? OData.Constants.DefaultMaxTop;
        OData.Constants.CurrentMaxTop = maxTop;

        var oDataBuilder = mvcBuilder.AddOData(options =>
        {
            options.Count().Filter().Select().OrderBy().Expand().SetMaxTop(maxTop);
            options.RouteOptions.EnableKeyInParenthesis = true;
            options.RouteOptions.EnableNonParenthesisForEmptyParameterFunction = true;
            //options.RouteOptions.EnablePropertyNameCaseInsensitive = true;
            options.RouteOptions.EnableQualifiedOperationCall = true;
            options.RouteOptions.EnableUnqualifiedOperationCall = true;
        });

        if (useVersioning == true)
        {
            AddODataVersioning<TModelConfiguration>(services, config, appManifest);
        }
    }

    private static IApiVersioningBuilder AddODataVersioning<TModelConfiguration>(IServiceCollection services, IConfiguration config, ApplicationManifest appManifest)
        where TModelConfiguration : IVersionedModelConfiguration, new()
    {
        return services
            .AddApiVersioning(options =>
            {
                // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                options.ReportApiVersions = true;
            })
            .AddOData(options =>
            {
                var useBatchMode = config.GetValue<bool?>("Scamark:API:OData:BatchModeEnabled") == true;

                if (useBatchMode)
                {
                    options.AddRouteComponents(RoutePrefix.Versioned, CreateODataBatchHandler(config));
                }
                else
                {
                    options.AddRouteComponents(RoutePrefix.Versioned);
                }

                var model = options.ModelBuilder.ModelConfigurations.OfType<ScamarkModelConfiguration>().FirstOrDefault();
                model.Value = new TModelConfiguration();

                options.ModelBuilder.DefaultModelConfiguration = (mb, version, s) =>
                {
                    var apiName = appManifest.Name;
                    mb.Namespace = $"Scamark.ODataClients.V{version?.MajorVersion?.ToString() ?? "1"}.{apiName}";

                    // pour la compatibilité avec les applications existantes
                    var useLegacyCase = config.GetValue<bool>("Scamark:API:OData:UseLegacyCase");

                    if (useLegacyCase == false)
                    {
                        // Le composant OData transforme la casse des propriétés en camelCase, ce qui peut
                        // avoir des effets indésirables. Ce fix permet de garder la casse originale de la propriété en C#
                        ((ODataConventionModelBuilder)mb).OnModelCreating += new DefaultCaser().Apply;
                    }
                };

            })
            .AddODataApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as "'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'V";

                // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
    }

    private static DefaultODataBatchHandler CreateODataBatchHandler(IConfiguration config)
    {
        // cf. https://devblogs.microsoft.com/odata/all-in-one-with-odata-batch/
        var odataBatchHandler = new DefaultODataBatchHandler();

        odataBatchHandler.MessageQuotas.MaxNestingDepth = config.GetValue<int?>("Scamark:API:OData:Quotas:MaxNestingDepth") ?? OData.Constants.DefaultBatchQuotaMaxNestingDepth;
        odataBatchHandler.MessageQuotas.MaxOperationsPerChangeset = config.GetValue<int?>("Scamark:API:OData:Quotas:MaxOperationsPerChangeset") ?? OData.Constants.DefaultBatchQuotaMaxOperationsPerChangeset;
        odataBatchHandler.MessageQuotas.MaxReceivedMessageSize = config.GetValue<long?>("Scamark:API:OData:Quotas:MaxReceivedMessageSize") ?? OData.Constants.DefaultBatchQuotaMaxReceivedMessageSize;

        return odataBatchHandler;
    }
}
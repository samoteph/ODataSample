using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.OData.ModelBuilder;
using Scamark.Microservice.OData.Builder;

namespace Scamark.Microservice.OData;

public sealed class ScamarkModelConfiguration : IModelConfiguration
{
    public IVersionedModelConfiguration Value { get; set; }

    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
    {
        if (routePrefix != RoutePrefix.Versioned)
        {
            return;
        }
        Value?.Apply(builder, apiVersion);
    }
}

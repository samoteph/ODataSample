using Asp.Versioning;
using Microsoft.OData.ModelBuilder;

namespace Scamark.Microservice.OData.Builder;

public interface IVersionedModelConfiguration
{
    //
    // Summary:
    //     Applies model configurations using the provided builder for the specified API
    //     version.
    //
    // Parameters:
    //   builder:
    //     The builder used to apply configurations.
    //
    //   apiVersion:
    //     The API version associated with the builder.
    void Apply(ODataModelBuilder builder, ApiVersion apiVersion);
}

using System;
using Asp.Versioning;
using Microsoft.OData.ModelBuilder;
using Scamark.Microservice.OData;
using Scamark.Microservice.OData.Builder;
using Scamark.Samples.Microservice.WebHost.V1.Models;

namespace Scamark.Samples.Microservice.WebHost;

public class ODataModelConfiguration : IVersionedModelConfiguration
{
    public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.EntitySet<Order>("Orders").EntityType.HasKey(x => x.Id);

        builder.EntitySet<Order>("Orders").EntityType.AddCollectionAction("BatchCreate")
        .Returns<BatchResult>().CollectionParameter<string>("value");

        var people = builder.EntitySet<People>("People").EntityType;
        people.HasKey(x => new { x.FirstName, x.LastName });
        //people.Function("MostExpensive").ReturnsFromEntitySet<People>("People");
        people.Collection.Function("MostExpensive").ReturnsFromEntitySet<People>("People");
    }
}

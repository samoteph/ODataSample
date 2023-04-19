// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.OData.ModelBuilder;

namespace Scamark.Microservice.OData;

public class DefaultCaser
{
    private readonly NameResolverOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="LowerCamelCaser"/> class.
    /// </summary>
    public DefaultCaser()
        : this(NameResolverOptions.ProcessReflectedPropertyNames |
            NameResolverOptions.ProcessDataMemberAttributePropertyNames |
            NameResolverOptions.ProcessExplicitPropertyNames)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LowerCamelCaser"/> class.
    /// </summary>
    /// <param name="options">Name resolver options for camelizing.</param>
    public DefaultCaser(NameResolverOptions options)
    {
        _options = options;
    }

    /// <summary>
    /// Applies lower camel case.
    /// </summary>
    /// <param name="builder">The <see cref="ODataConventionModelBuilder"/> to be applied on.</param>
    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Explicit Expression generic type is more clear")]
    public void Apply(ODataConventionModelBuilder builder)
    {
        foreach (var typeConfiguration in builder.StructuralTypes)
        {
            foreach (var property in typeConfiguration.Properties)
            {
                if (ShouldApply(property))
                {
                    property.Name = property.PropertyInfo.Name;
                }
            }
        }
    }

    private bool ShouldApply(PropertyConfiguration property)
    {
        if (property.AddedExplicitly)
        {
            return _options.HasFlag(NameResolverOptions.ProcessExplicitPropertyNames);
        }
        else
        {
            var attribute = property.PropertyInfo.GetCustomAttribute<DataMemberAttribute>(inherit: false);

            if (attribute != null && !string.IsNullOrWhiteSpace(attribute.Name))
            {
                return _options.HasFlag(NameResolverOptions.ProcessDataMemberAttributePropertyNames);
            }

            return _options.HasFlag(NameResolverOptions.ProcessReflectedPropertyNames);
        }
    }
}

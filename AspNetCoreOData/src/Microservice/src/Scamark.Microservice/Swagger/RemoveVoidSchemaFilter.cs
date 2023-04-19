using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Scamark.Microservice.Swagger;

internal class RemoveVoidSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        context.SchemaRepository.Schemas.Remove("Void");
    }
}

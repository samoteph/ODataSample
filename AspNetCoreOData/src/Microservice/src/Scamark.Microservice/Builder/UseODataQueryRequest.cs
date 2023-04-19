using Microsoft.AspNetCore.Builder;
using Scamark.Microservice.OData;

namespace Scamark.Microservice;

public static class UseODataQueryRequest
{
    /// <summary>
    /// Use OData query request middleware. An OData query request is a Http Post request ending with /$query.
    /// The Request body contains the query options.
    /// </summary>
    /// <param name="app">The <see cref="IApplicationBuilder "/> to use.</param>
    /// <returns>The <see cref="IApplicationBuilder "/>.</returns>
    public static IApplicationBuilder UseCustomODataQueryRequest(this IApplicationBuilder app)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        return app.UseMiddleware<ODataQueryRequestMiddleware>();
    }
}

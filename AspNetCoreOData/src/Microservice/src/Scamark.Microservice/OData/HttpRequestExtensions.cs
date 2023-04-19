using Microsoft.AspNetCore.Http;

namespace Scamark.Microservice.OData;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Checks whether the request is a POST targeted at a resource path ending in /$query.
    /// </summary>
    /// <param name="request">The <see cref="HttpRequest"/> instance to extend.</param>
    /// <returns>true if the request path has $query segment.</returns>
    internal static bool IsODataQueryRequest(this HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        // Requests to paths ending in /$query MUST use the POST verb.
        if (!string.Equals(request.Method, HttpMethods.Post, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string path = request.Path.Value.TrimEnd('/');
        return path.EndsWith("/$query", StringComparison.OrdinalIgnoreCase);
    }
}

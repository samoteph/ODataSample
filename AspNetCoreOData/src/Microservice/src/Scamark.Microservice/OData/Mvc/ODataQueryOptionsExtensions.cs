using Microsoft.AspNetCore.OData.Query;

namespace Scamark.Microservice.OData.Mvc;

public static class ODataQueryOptionsExtensions
{
    public static IDictionary<string, string> ToODataQueryDictionary(this ODataQueryOptions options)
    {
        // Pour le moment, par simplicité on récupère les valeurs depuis le query string,
        // mais idéalement il faudrait les récupérer depuis les options (car les paramètres OData ont été validés au préalable)
        return options.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
    }
}

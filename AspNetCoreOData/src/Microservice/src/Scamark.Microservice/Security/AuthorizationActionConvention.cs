using Asp.Versioning.Controllers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Routing;
using Scamark.Microservice.Authorization;

namespace Scamark.Microservice.Security;

public class AuthorizationActionConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        if (action is null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (action.Controller.ControllerType == typeof(VersionedMetadataController))
        {
            action.Filters.Add(new AuthorizeFilter(policy: ApiPolicy.ScamarkMetadataRead.ToString()));
            return;
        }

        ////Console.WriteLine($"{action.ActionName} => {action.Controller.DisplayName} {action.ActionMethod}  ");

        // c'est plus fiable d'utiliser les métadonnées que les attributs, puisque l'attribut sur l'action n'est pas obligatoire
        var httpMethods = action.Selectors.SelectMany(x => x.EndpointMetadata).OfType<HttpMethodMetadata>().SelectMany(x => x.HttpMethods).ToArray();
        var isReadAction = httpMethods.All(x => x == "GET" || x == "OPTIONS" || x == "CONNECT");

        // Require specific claims for mutable actions    
        if (isReadAction == false)
        {
            action.Filters.Add(new AuthorizeFilter(policy: ApiPolicy.SapWrite.ToString()));
        }
        else
        {
            action.Filters.Add(new AuthorizeFilter(policy: ApiPolicy.ScamarkRead.ToString()));
        }
    }
}

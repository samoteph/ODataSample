using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Scamark.Microservice.OData.Mvc;

/// <summary>
/// Controlleur de base OData dont la route est /{version}/[controller]
/// </summary>
[Produces("application/json")]
public abstract class ScamarkODataController : ODataController
{
    public const string VersionedRoutePrefix = "v{version:apiVersion}";

    /// <summary>
    /// Returns a single result from a query (it has to be a query to enable OData $filter $expand etc)
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    /// <param name="queryable">The System.Linq.IQueryable`1 containing zero or one entities.</param>
    /// <returns>A new instance of <see cref="Microsoft.AspNet.OData.SingleResult"/></returns>
    protected static SingleResult<T> SingleResult<T>(IQueryable<T> queryable)
    {
        return Microsoft.AspNetCore.OData.Results.SingleResult.Create(queryable);
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Scamark.Microservice.OData.Mvc;
using Scamark.Samples.Microservice.WebHost.V1.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Microsoft.AspNetCore.OData.Query.AllowedQueryOptions;

namespace Scamark.Samples.Microservice.WebHost.V1;

[ApiVersion(1.0)]
public class PeopleController : ScamarkODataController
{
    [HttpGet]
    [ProducesResponseType(typeof(ODataValue<IEnumerable<People>>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count)]
    public IQueryable<People> Get()
    {
        return GetEntities();
    }

    /// <summary>
    /// Gets a single order.
    /// </summary>
    /// <param name="key">The requested order identifier.</param>
    /// <returns>The requested order.</returns>
    /// <response code="200">The order was successfully retrieved.</response>
    /// <response code="404">The order does not exist.</response>
    [HttpGet]
    // https://localhost:5001/v1/People(FirstName='Adrien',LastName='Constant')
    [ProducesResponseType(typeof(People), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [EnableQuery(AllowedQueryOptions = Select)]
    public SingleResult<People> Get(string keyFirstName, string keyLastName)
    {
        return SingleResult(GetEntities().Where(x => x.FirstName == keyFirstName && x.LastName == keyLastName));
    }

    /// <summary>
    /// Gets the most expensive person.
    /// </summary>
    /// <returns>The most expensive person.</returns>
    /// <response code="200">The person was successfully retrieved.</response>
    /// <response code="404">No people exist.</response>
    [HttpGet]
    [MapToApiVersion(1.0)]
    [Produces("application/json")]
    [ProducesResponseType(typeof(People), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [EnableQuery(AllowedQueryOptions = Select)]
    public SingleResult<People> MostExpensive(ODataQueryOptions<People> options, CancellationToken ct)
    {
        var query = new People[]
        {
            new()
            {
                FirstName = "Elon",
                LastName = "Musk",
            }
        }.AsQueryable();

        return SingleResult(query);
    }

    private IQueryable<People> GetEntities()
    {
        return new[] { new People { FirstName = "Adrien", LastName = "Constant" } }.AsQueryable();
    }
}

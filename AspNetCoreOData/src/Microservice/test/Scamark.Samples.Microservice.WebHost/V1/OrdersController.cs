using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Versioning;
using Asp.Versioning.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Scamark.Microservice.OData;
using Scamark.Microservice.OData.Mvc;
using Scamark.Samples.Microservice.WebHost.V1.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Microsoft.AspNetCore.OData.Query.AllowedQueryOptions;

namespace Scamark.Samples.Microservice.WebHost.V1;

[ApiVersion(1.0)]
public class OrdersController : ScamarkODataController
{
    [ProducesResponseType(typeof(ODataValue<IEnumerable<Order>>), Status200OK)]
    [EnableQuery(MaxTop = 100, AllowedQueryOptions = Select | Top | Skip | Count)]
    public IQueryable<Order> Get()
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
    [ProducesResponseType(typeof(Order), Status200OK)]
    [ProducesResponseType(Status404NotFound)]
    [EnableQuery(AllowedQueryOptions = Select)]
    public SingleResult<Order> Get(int key)
    {
        return SingleResult(GetEntities().Where(x => x.Id == key));
    }

    public IActionResult Post([FromBody] Order model)
    {
        model.Id = 1;
        return Created(model);
    }


    [HttpPost]
    public async Task<IActionResult> BatchCreate([FromBody] ODataActionParameters parameters)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var entities = parameters.Convert<ODataCollectionParameter<string>>();
        return Ok(await Task.FromResult(new BatchResult { Count = entities.Value.Count() }));
    }

    private IQueryable<Order> GetEntities()
    {
        return Enumerable.Range(0, 9)
            .Select(i => new Order() { Id = 42 + i, Description = $"Moutons électriques Number {i}" })
            .AsQueryable();
    }
}

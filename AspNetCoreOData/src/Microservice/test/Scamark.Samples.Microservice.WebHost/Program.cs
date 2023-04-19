using System.Text;
using Microsoft.AspNetCore.Builder;
#if !OSX
using Scamark.Framework.Common;
using Scamark.Framework.LinqToDB;
#endif
using Scamark.Microservice;
using Scamark.Samples.Microservice.WebHost;

//todo: ne pas avoir à passer deux fois le flag
bool useVersioning = true;

var appBuilder = ScamarkHostBuilder.CreateODataWebHost<ODataModelConfiguration>(args, useVersioning: useVersioning);

#if !OSX
appBuilder.Services.AddScamarkDbContext<HanaDbContext>(new DatabaseConfiguration
{
    ConnectionString = "Server=qua-hana-db.scamarknt.com:31013;Port=31013;Database=QF1;UserID=SVCHANAQF1;Password=hxd@7Aa4TK2b2wc^;IsMultitenant=1;IsInstanceNumber=10;CurrentSchema=QUAQF1",
    ServerType = DatabaseServerType.Hana
});
#endif

var app = appBuilder.Build();

app.UseScamarkOData(useVersioning: useVersioning);

app.MapGet("/", async req =>
{
    var buffer = Encoding.ASCII.GetBytes(@"
        <h2>hello world</h2>
        <li><a href=""/health"">health check</a>
        </li><li><a href=""/v1/$metadata"">OData Metadata</a></li>
        <li><a href=""/swagger/"">Swagger</a></li>
        <li><a href=""/v1/Orders/"">Orders (OData)</a></li>
        <li><a href=""/v1/People(FirstName='Adrien',LastName='Constant')"">People With (OData Multiple Keys with parenthesis)</a></li>
        <li><a href=""/v1/People/FirstName='Adrien',LastName='Constant'"">People With (OData Multiple Keys with slash)</a></li>
        <li><a href=""/$odata"">OData endpoints debug</a></li>");

    req.Response.ContentType = "text/html";
    await req.Response.Body.WriteAsync(buffer);
});

app.Run();

using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Scamark.Samples.Microservice.WebHost;

public sealed class HanaDbContext : DataConnection
{
    public HanaDbContext(LinqToDbConnectionOptions<HanaDbContext> options) : base(options)
    {
    }
}

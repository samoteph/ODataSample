using System.Text.Json.Serialization;

namespace Scamark.Microservice.OData;

public abstract class ODataObjectParameter
{
    [JsonIgnore]
    public object ObjectValue { get; set; }
}
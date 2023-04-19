using System.Text.Json;

namespace Scamark.Microservice;

public static class JsonDefaultSerializerOptions
{
    public static JsonSerializerOptions Options => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
}

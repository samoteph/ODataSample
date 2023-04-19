using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData;

namespace Scamark.Microservice.OData;

public class JsonODataQueryRequestParser : IODataQueryRequestParser
{
    private static readonly MediaTypeHeaderValue SupportedMediaType = MediaTypeHeaderValue.Parse("application/json");

    public bool CanParse(HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        return request.ContentType?.StartsWith(SupportedMediaType.MediaType, StringComparison.Ordinal) == true;
    }

    public async Task<string> ParseAsync(HttpRequest request)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        try
        {
            Stream requestStream = request.Body;
            using var reader = new StreamReader(requestStream, encoding: Encoding.UTF8);

            var content = await reader.ReadToEndAsync();
            var jsonNode = JsonNode.Parse(content);
            var jsonObject = jsonNode.AsObject();

            var result = $"({string.Join(",", jsonObject.Select(o => o.Key + "=" + (o.Value ?? "null")))})";

            return result;
        }
        catch
        {
            throw new ODataException("Unable to parse query request payload.");
        }
    }
}

using Microsoft.OData.UriParser;

namespace Scamark.Microservice.OData;

public class CaseInsensitiveResolver : ODataUriResolver
{
    public override bool EnableCaseInsensitive
    {
        get => true;
        set { }
    }
}

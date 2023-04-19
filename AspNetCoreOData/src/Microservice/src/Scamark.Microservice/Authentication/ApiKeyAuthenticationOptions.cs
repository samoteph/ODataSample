using Microsoft.AspNetCore.Authentication;

namespace Scamark.Microservice.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public ApiKeyAuthenticationOptions()
    {
        Scheme = ApiKeyConstants.DefaultScheme;
        AuthenticationType = ApiKeyConstants.DefaultScheme;
    }

    public string Scheme { get; set; }
    public string AuthenticationType { get; set; }
}

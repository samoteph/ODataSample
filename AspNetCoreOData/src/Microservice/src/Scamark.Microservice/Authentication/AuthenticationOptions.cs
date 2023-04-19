namespace Scamark.Microservice.Authentication;

public class AuthenticationOptions
{
    public const string ConfigSectionKey = "Scamark:API:Authentication";
    public bool Disabled { get; set; }
    public IEnumerable<ApiKey> ApiKeys { get; set; }
}

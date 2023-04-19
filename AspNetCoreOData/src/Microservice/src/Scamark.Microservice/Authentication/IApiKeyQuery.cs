namespace Scamark.Microservice.Authentication;

public interface IApiKeyQuery
{
    ApiKey Execute(string providedApiKey);
}

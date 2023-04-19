namespace Scamark.Microservice.Authentication;

public class ApiKey
{
    public static DateTime DefaultStartDate = new(2020, 07, 01);

    public ApiKey()
    {
        StartDate = DefaultStartDate;
    }

    public ApiKey(string owner, string key, IReadOnlyCollection<string> roles)
    {
        ClientId = owner ?? throw new ArgumentNullException(nameof(owner));
        ClientSecret = key ?? throw new ArgumentNullException(nameof(key));
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));
    }

    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public IReadOnlyCollection<string> Roles { get; set; }
}

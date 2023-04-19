namespace Scamark.Microservice;

public sealed class OpenApiConfiguration
{
    public OpenApiConfiguration(string name, string title = null, string description = null, string license = null,
        string contactName = null, string contactEmail = null, Uri contactUrl = null, string appVersion = null)
    {
        Name = name;
        Title = title ?? $"{Name} API";
        Description = description;
        License = license ?? "© Scamark 2022 - API cannot be used without explicit consent";
        ContactName = contactName ?? "Projet git";
        ContactUrl = contactUrl ?? new Uri($"https://scamark.visualstudio.com/API/_git/{name}");
        ContactEmail = contactEmail ?? "haroun.bellahcene@scamark.fr";
        AppVersion = appVersion;
        Description += $"<br>Version de l'application: <b>{AppVersion}</b>";
    }

    public string Name { get; }
    public string Title { get; }
    public string Description { get; }
    public string License { get; }
    public string ContactName { get; }
    public string ContactEmail { get; }
    public Uri ContactUrl { get; }
    public string AppVersion { get; }
}

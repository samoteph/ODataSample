using Microsoft.AspNetCore.Http;

namespace Scamark.Microservice;

/// <summary>
/// Ce middleware permet d'exposer l'URL /Debug/ViewHeaders pour afficher les valeurs du headers du serveur
/// </summary>
public class DebugMiddleware
{
    private readonly RequestDelegate _next;

    public DebugMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Request.Path == "/Debug/HttpRequestHeaders" && context.Request.Method == "GET")
        {
            context.Response.ContentType = "text/plain";
            foreach (var header in context.Request.Headers)
            {
                await context.Response.WriteAsync(header.Key + " = " + header.Value.ToString() + Environment.NewLine);
            }
            return;
        }
        else if (context.Request.Path == "/Debug/EnvVariables" && context.Request.Method == "GET")
        {
            context.Response.ContentType = "text/plain";
            var variables = Environment.GetEnvironmentVariables();
            foreach (var variableKey in variables.Keys)
            {
                await context.Response.WriteAsync(variableKey + " = " + variables[variableKey] + Environment.NewLine);
            }
            return;
        }
        await _next(context);
    }
}

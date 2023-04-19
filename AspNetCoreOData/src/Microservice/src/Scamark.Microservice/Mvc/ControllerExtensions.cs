using System.IO;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Scamark.Microservice.Mvc;

public static class ControllerExtensions
{
    /// <summary>
    /// Permet de parser le Body de la requête sous forme de JSON vers le type de l'objet
    /// spécifié.
    /// </summary>
    /// <remarks>
    /// Le mode Buffering doit être activté sur la requête au préalable.
    /// <code>
    /// app.Use(async (context, next) =>
    /// {
    ///      context.Request.EnableBuffering();
    ///      await next();
    /// });
    /// </code>
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static async Task<T> ParseJsonBody<T>(this Controller source)
    {
        var body = source.HttpContext.Request.Body;
        body.Seek(0, SeekOrigin.Begin);
        var model = await JsonSerializer.DeserializeAsync<T>(
            body,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        return model;
    }
}
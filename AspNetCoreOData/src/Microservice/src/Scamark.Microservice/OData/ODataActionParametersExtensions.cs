using System.Text.Json;

namespace Microsoft.AspNetCore.OData.Formatter;

public static class ODataActionParametersExtensions
{
    /// <summary>
    /// Permet de convertir <see cref="ODataActionParameters"/> en la classe spécifiée.
    /// Utilise automapper dynamiquement.
    /// </summary>
    /// <typeparam name="T">Le type de la destination.</typeparam>
    /// <param name="source">L'objet <see cref="ODataActionParameters"/> source.</param>
    /// <returns>Une nouvelle instance de type <typeparam name="T"> créée à partir des valeurs du dictionnaire <paramref name="source"/>.</returns>
    public static T Convert<T>(this ODataActionParameters source) where T : new()
    {
        var instance = Activator.CreateInstance<T>();
        foreach (var parameter in source)
        {
            var instanceProperty = typeof(T).GetProperties().FirstOrDefault(x => x.Name.ToLower() == parameter.Key.ToLower());

            if (instanceProperty == null)
            {
                continue;
            }
            var json = JsonSerializer.Serialize(parameter.Value);
            var obj = JsonSerializer.Deserialize(json, instanceProperty.PropertyType);
            instanceProperty.SetValue(instance, obj);
        }

        return instance;
    }
}
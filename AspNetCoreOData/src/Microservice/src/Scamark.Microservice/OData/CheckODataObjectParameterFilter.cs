using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Scamark.Microservice.OData;

/// <summary>
/// Ce filtre permet de retourner <see cref="NotFoundResult"/> dans un controlleur,
/// lorsque la méthode prend en paramètre un objet de type <see cref="ODataObjectParameter"/>
/// et que celui-ci est <see cref="null"/>.
/// </summary>
public class CheckODataObjectParameterFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        bool isMatchingParameter(ParameterDescriptor x)
        {
            return typeof(ODataObjectParameter).IsAssignableFrom(x.ParameterType)
                && context.ActionArguments.TryGetValue(x.Name, out var parameterValue) == true
                && ((ODataObjectParameter)parameterValue)?.ObjectValue == null;
        }

        if (context.ActionDescriptor.Parameters.Any(isMatchingParameter))
        {
            context.Result = new NotFoundResult();
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
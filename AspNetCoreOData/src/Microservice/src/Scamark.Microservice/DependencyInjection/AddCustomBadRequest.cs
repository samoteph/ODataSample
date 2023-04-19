using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Scamark.Microservice.Mvc;

namespace Scamark.Microservice;

public static partial class ServiceCollectionExtensions
{
    public static void AddCustomBadRequest(IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
           {
               var problemDetails = new CustomBadRequest(context);
               return new BadRequestObjectResult(problemDetails)
               {
                   ContentTypes = { "application/problem+json", "application/problem+xml" }
               };
           };
        });
    }
}

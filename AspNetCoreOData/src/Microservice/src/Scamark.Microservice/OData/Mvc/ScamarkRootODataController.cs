using Microsoft.AspNetCore.Mvc;

namespace Scamark.Microservice.OData.Mvc;

/// <summary>
/// Controlleur de base OData dont la route est /v{version:apiVersion}/
/// </summary>
[Route(VersionedRoutePrefix)]
public abstract class ScamarkRootODataController : ScamarkODataController
{
}

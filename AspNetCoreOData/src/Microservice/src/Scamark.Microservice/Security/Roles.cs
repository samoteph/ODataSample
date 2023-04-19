namespace Scamark.Microservice.Security;

public static class Roles
{
    public const string ApiScamarkMetadataRead = "Api.Scamark.Metadata.Read";
    public const string ApiScamarkRead = "Api.Scamark.Read";
    public const string ApiSapRead = "Api.Sap.Read";
    public const string ApiScamarkWrite = "Api.Scamark.Write";
    public const string ApiSapWrite = "Api.Sap.Write";
    public static readonly string[] AllRoles = new[] { ApiSapRead, ApiSapWrite, ApiScamarkMetadataRead, ApiScamarkRead, ApiScamarkWrite };
}

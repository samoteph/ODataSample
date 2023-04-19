namespace Scamark.Microservice.OData;

internal class Constants
{
    internal const long DefaultBatchQuotaMaxReceivedMessageSize = 20 * 1000 * 1000; // 20 Mb
    internal const int DefaultBatchQuotaMaxNestingDepth = 10;
    internal const int DefaultBatchQuotaMaxOperationsPerChangeset = 10000;
    public const int DefaultMaxTop = 1000;
    internal static int CurrentMaxTop = DefaultMaxTop;
}

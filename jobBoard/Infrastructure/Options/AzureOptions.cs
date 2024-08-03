namespace JobBoard.Infrastructure.Options;

public class AzureOptions
{
    public const string Key = "Azure";

    public required string StorageConnectionString { get; init; }
    public required string BusinessLogoContainer { get; init; }
}
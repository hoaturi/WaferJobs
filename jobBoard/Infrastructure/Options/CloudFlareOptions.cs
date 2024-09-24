namespace JobBoard.Infrastructure.Options;

public class CloudFlareOptions
{
    public const string Key = "CloudFlare";

    public required string AccessKeyId { get; init; }
    public required string SecretAccessKey { get; init; }
    public required string S3Endpoint { get; init; }
    public required string S3BucketName { get; init; }
    public required string BaseUrl { get; init; }
}
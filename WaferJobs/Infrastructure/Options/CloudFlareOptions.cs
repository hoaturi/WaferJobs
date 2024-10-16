using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class CloudFlareOptions
{
    public const string Key = "CloudFlare";

    [Required] public required string AccessKeyId { get; init; }

    [Required] public required string SecretAccessKey { get; init; }

    [Required] public required string S3Endpoint { get; init; }

    [Required] public required string S3BucketName { get; init; }

    [Required] public required string ImageBaseUrl { get; init; }
}
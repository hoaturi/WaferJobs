using System.ComponentModel.DataAnnotations;

namespace WaferJobs.Infrastructure.Options;

public class RedisOptions
{
    public const string Key = "Redis";
    [Required] public required string Host { get; init; }
    [Required] public required string Port { get; init; }
    [Required] public required string Password { get; init; }
}
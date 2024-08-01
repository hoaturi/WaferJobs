using System.ComponentModel.DataAnnotations;

namespace JobBoard.Common.Options;

public class RedisOptions
{
    public const string Key = "Redis";
    [Required] public required string Host { get; init; }
    [Required] public required string Port { get; init; }
    [Required] public string ConnectionString => $"{Host}:{Port}";
}
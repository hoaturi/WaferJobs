using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public record class RedisOptions
{
    public const string Key = "Redis";

    [Required]
    public required string Host { get; init; }

    [Required]
    public required string Port { get; init; }

    public string ConnectionString => $"{Host}:{Port}";
}

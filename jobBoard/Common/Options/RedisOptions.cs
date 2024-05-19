namespace JobBoard.Common.Options;

public class RedisOptions
{
    public const string Key = "Redis";

    public required string Host { get; init; }

    public required string Port { get; init; }

    public string ConnectionString => $"{Host}:{Port}";
}
using MessagePack;

namespace JobBoard.Domain.Common;

[MessagePackObject]
public class CountryEntity
{
    [Key(0)] public int Id { get; set; }

    [Key(1)] public required string Label { get; init; }

    [Key(2)] public required string Code { get; init; }

    [Key(3)] public required string Slug { get; init; }
}
using MessagePack;

namespace JobBoard.Features.Lookup.GetPopularKeywords;

[MessagePackObject]
public record PopularKeywordDto(
    [property: Key(0)] string Keyword,
    [property: Key(1)] int Count
);
namespace JobBoard.Features.Lookup.GetPopularKeywords;

public record PopularKeywordDto(
    string Label,
    int Count
);

public record GetPopularKeywordsResponse(
    List<PopularKeywordDto> Keywords
);
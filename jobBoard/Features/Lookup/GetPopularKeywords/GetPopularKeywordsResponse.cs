namespace JobBoard.Features.Lookup.GetPopularKeywords;

public record GetPopularKeywordsResponse(
    List<PopularKeywordDto> Keywords
);
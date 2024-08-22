using JobBoard.Infrastructure.Services.CachingServices.PopularKeywordsService;

namespace JobBoard.Features.Lookup.GetPopularKeywords;

public record GetPopularKeywordsResponse(
    IReadOnlyList<PopularKeywordDto> Keywords
);
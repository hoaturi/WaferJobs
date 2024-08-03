using JobBoard.Infrastructure.Services.LookupServices.PopularKeywordsService;

namespace JobBoard.Features.Lookup.GetPopularKeywords;

public record GetPopularKeywordsResponse(
    IReadOnlyList<PopularKeywordDto> Keywords
);
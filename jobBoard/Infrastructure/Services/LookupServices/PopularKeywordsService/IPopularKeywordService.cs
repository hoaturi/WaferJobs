using JobBoard.Features.Lookup.GetPopularKeywords;

namespace JobBoard.Infrastructure.Services.LookupServices.PopularKeywordsService;

public interface IPopularKeywordsService
{
    Task<IReadOnlyList<PopularKeywordDto>> GetPopularKeywordsAsync(CancellationToken cancellationToken);
}
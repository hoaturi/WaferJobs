namespace JobBoard.Infrastructure.Services.CachingServices.PopularKeywordsService;

public interface IPopularKeywordsService
{
    Task<IReadOnlyList<PopularKeywordDto>> GetPopularKeywordsAsync(CancellationToken cancellationToken);
}
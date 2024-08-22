using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.CachingServices.PopularKeywordsService;

public class PopularKeywordsService(
    AppDbContext dbContext,
    IDistributedCache cache,
    ILogger<PopularKeywordsService> logger) : IPopularKeywordsService
{
    private static readonly HashSet<string> CommonWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "the", "in", "on", "at", "to", "for", "of", "with", "by", "from", "up", "about", "into",
        "over", "after"
    };

    public async Task<IReadOnlyList<PopularKeywordDto>> GetPopularKeywordsAsync(CancellationToken cancellationToken)
    {
        var keywords = await cache.GetAsync(CacheKeys.PopularKeywords, cancellationToken);

        if (keywords is not null)
            return MessagePackSerializer.Deserialize<IReadOnlyList<PopularKeywordDto>>(keywords
                , cancellationToken: cancellationToken);

        var popularKeywords = await FetchAndProcessPopularKeywords(cancellationToken);
        await CachePopularKeywords(popularKeywords, cancellationToken);

        return popularKeywords;
    }

    private async Task<IReadOnlyList<PopularKeywordDto>> FetchAndProcessPopularKeywords(
        CancellationToken cancellationToken)
    {
        var tagLabels = await dbContext.JobPosts
            .AsNoTracking()
            .SelectMany(jp => jp.Tags.Select(t => t.Label))
            .ToListAsync(cancellationToken);

        return tagLabels
            .Where(label => !string.IsNullOrWhiteSpace(label) && !CommonWords.Contains(label))
            .GroupBy(label => label)
            .Select(g => new PopularKeywordDto(g.Key, g.Count()))
            .OrderByDescending(g => g.Count)
            .Take(15)
            .ToList();
    }

    private async Task CachePopularKeywords(IReadOnlyList<PopularKeywordDto> popularKeywords,
        CancellationToken cancellationToken)
    {
        var serializedKeywords = MessagePackSerializer.Serialize(popularKeywords, cancellationToken: cancellationToken);
        await cache.SetAsync(CacheKeys.PopularKeywords, serializedKeywords, cancellationToken);
        logger.LogInformation("Updated popular keywords cache");
    }
}
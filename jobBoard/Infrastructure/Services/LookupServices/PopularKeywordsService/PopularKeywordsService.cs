using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.LookupServices.PopularKeywordsService;

public class PopularKeywordsService(AppDbContext dbContext, IDistributedCache cache, ILogger<PopularKeywordsService> logger) : IPopularKeywordsService
{
    private static readonly HashSet<string> CommonWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "the", "in", "on", "at", "to", "for", "of", "with", "by", "from", "up", "about", "into",
        "over", "after"
    };

    public async Task<IReadOnlyList<PopularKeywordDto>> GetPopularKeywordsAsync(CancellationToken cancellationToken)
    {
        var keywords = await cache.GetAsync(CacheKeys.PopularKeywordsCacheKey, cancellationToken);

        if (keywords is not null)
            return MessagePackSerializer.Deserialize<IReadOnlyList<PopularKeywordDto>>(keywords,
                cancellationToken: cancellationToken);

        var popularKeywords = await FetchAndProcessPopularKeywords(cancellationToken);
        await CachePopularKeywords(popularKeywords, cancellationToken);

        return popularKeywords;
    }

    private async Task<IReadOnlyList<PopularKeywordDto>> FetchAndProcessPopularKeywords(
        CancellationToken cancellationToken)
    {
        var titles = await dbContext.JobPosts
            .Select(jp => jp.Title)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return titles
            .SelectMany(title => title.Split(' '))
            .Select(word => word.ToLowerInvariant())
            .Where(word => word.Length > 1 && !CommonWords.Contains(word))
            .GroupBy(word => word)
            .Select(g => new PopularKeywordDto(g.Key, g.Count()))
            .OrderByDescending(g => g.Count)
            .Take(15)
            .ToList();
    }

    private async Task CachePopularKeywords(IReadOnlyList<PopularKeywordDto> popularKeywords,
        CancellationToken cancellationToken)
    {
        var serializedKeywords = MessagePackSerializer.Serialize(popularKeywords, cancellationToken: cancellationToken);
        await cache.SetAsync(CacheKeys.PopularKeywordsCacheKey, serializedKeywords, cancellationToken);
        logger.LogInformation("Updated popular keywords cache");
        
    }
}
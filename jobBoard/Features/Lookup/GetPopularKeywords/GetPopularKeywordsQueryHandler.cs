using JobBoard.Common.Constants;
using JobBoard.Common.Models;
using JobBoard.Infrastructure.Persistence;
using MediatR;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Features.Lookup.GetPopularKeywords;

public class GetPopularKeywordsQueryHandler(
    AppDbContext dbContext,
    IDistributedCache cache
) : IRequestHandler<GetPopularKeywordsQuery, Result<GetPopularKeywordsResponse, Error>>
{
    private static readonly HashSet<string> CommonWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "the", "in", "on", "at", "to", "for", "of", "with", "by", "from", "up", "about", "into",
        "over", "after"
    };

    public async Task<Result<GetPopularKeywordsResponse, Error>> Handle(GetPopularKeywordsQuery query,
        CancellationToken cancellationToken)
    {
        var keywords = await cache.GetAsync(CacheKeys.PopularKeywordsCacheKey, cancellationToken);

        if (keywords is null)
        {
            var titles = await dbContext.JobPosts
                .Select(jp => jp.Title)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var popularKeywords = titles
                .SelectMany(title => title.Split(' '))
                .Select(word => word.ToLowerInvariant())
                .Where(word => word.Length > 1 && !CommonWords.Contains(word))
                .GroupBy(word => word)
                .Select(g => new PopularKeywordDto(g.Key, g.Count()))
                .OrderByDescending(g => g.Count)
                .Take(15)
                .ToList();


            var serializedKeywords =
                MessagePackSerializer.Serialize(popularKeywords, cancellationToken: cancellationToken);

            await cache.SetAsync(CacheKeys.PopularKeywordsCacheKey, serializedKeywords, cancellationToken);

            return new GetPopularKeywordsResponse(popularKeywords);
        }

        var deserializedKeywords =
            MessagePackSerializer.Deserialize<List<PopularKeywordDto>>(keywords, cancellationToken: cancellationToken);

        return new GetPopularKeywordsResponse(deserializedKeywords);
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;
using WaferJobs.Common.Models;
using WaferJobs.Infrastructure.Persistence;

namespace WaferJobs.Features.Lookup.GetPopularKeywords;

public class GetPopularKeywordsQueryHandler(AppDbContext dbContext)
    : IRequestHandler<GetPopularKeywordsQuery, Result<GetPopularKeywordsResponse, Error>>
{
    private static readonly HashSet<string> CommonWords = new(StringComparer.OrdinalIgnoreCase)
    {
        "a", "an", "and", "the", "in", "on", "at", "to", "for", "of", "with", "by", "from", "up", "about", "into",
        "over", "after"
    };

    public async Task<Result<GetPopularKeywordsResponse, Error>> Handle(GetPopularKeywordsQuery query,
        CancellationToken cancellationToken)
    {
        var popularKeywords = await FetchPopularKeywords(cancellationToken);
        return new GetPopularKeywordsResponse(popularKeywords);
    }

    private async Task<List<PopularKeywordDto>> FetchPopularKeywords(CancellationToken cancellationToken)
    {
        return await dbContext.JobPosts
            .AsNoTracking()
            .SelectMany(jp => jp.Tags.Select(t => t.Label))
            .Where(label => !string.IsNullOrWhiteSpace(label) && !CommonWords.Contains(label))
            .GroupBy(label => label)
            .Select(g => new { Label = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count)
            .Take(15)
            .Select(g => new PopularKeywordDto(g.Label, g.Count))
            .ToListAsync(cancellationToken);
    }
}
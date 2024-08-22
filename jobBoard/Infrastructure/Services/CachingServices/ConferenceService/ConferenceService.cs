using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.CachingServices.ConferenceService;

public class ConferenceService(AppDbContext dbContext, IDistributedCache cache) : IConferenceService
{
    public async Task<IReadOnlyList<ConferenceDto>> GetConferencesAsync(CancellationToken cancellationToken)
    {
        var conferences = await cache.GetAsync(CacheKeys.Conferences, cancellationToken);

        if (conferences is not null)
            return MessagePackSerializer.Deserialize<IReadOnlyList<ConferenceDto>>(conferences,
                cancellationToken: cancellationToken);

        return await RefreshConferencesCacheAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ConferenceDto>> RefreshConferencesCacheAsync(CancellationToken cancellationToken)
    {
        var conferences = await GetConferencesFromDb(cancellationToken);

        var serializedConferences = MessagePackSerializer.Serialize(conferences, cancellationToken: cancellationToken);

        await cache.SetAsync(CacheKeys.Conferences, serializedConferences, cancellationToken);

        return conferences;
    }

    private async Task<List<ConferenceDto>> GetConferencesFromDb(CancellationToken cancellationToken)
    {
        return await dbContext.Conferences
            .AsNoTracking()
            .Where(c => c.IsPublished && c.StartDate > DateTime.UtcNow)
            .Select(c => new ConferenceDto(
                c.Title,
                c.OrganizerName,
                c.OrganizerEmail,
                c.Location,
                c.WebsiteUrl,
                c.StartDate,
                c.EndDate
            ))
            .ToListAsync(cancellationToken);
    }
}
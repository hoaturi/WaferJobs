using JobBoard.Common.Constants;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace JobBoard.Infrastructure.Services.JobMetricService;

public class JobMetricService(
    AppDbContext dbContext,
    IConnectionMultiplexer redis,
    IHttpContextAccessor httpContext,
    ILogger<JobMetricService> logger)
    : IJobMetricService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private readonly IServer _server = redis.GetServer(redis.GetEndPoints().First());

    public async Task IncrementApplyCountAsync(Guid jobId, CancellationToken cancellationToken)
    {
        var ipAddress = httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ipAddress)) return;
        if (await IsAlreadyApplied(ipAddress, jobId, cancellationToken)) return;

        var cacheKey = $"{CacheKeys.ApplyClickCount}:{jobId}";
        var cacheKeyWithIp = $"{CacheKeys.ApplyClickCount}:{jobId}:{ipAddress}";

        await _db.StringIncrementAsync(cacheKey);
        await _db.StringSetAsync(cacheKeyWithIp, 1, TimeSpan.FromDays(1));
    }

    public async Task PersistApplyCountAsync(CancellationToken cancellationToken)
    {
        var keys = _server.Keys(0, $"{CacheKeys.ApplyClickCount}:*").ToArray();

        if (keys.Length == 0) return;

        var cacheList = await _db.StringGetAsync(keys);

        var jobIds = keys
            .Select(k => k.ToString().Split(":").Last())
            .Where(id => Guid.TryParse(id, out _))
            .Select(Guid.Parse)
            .ToList();

        var cacheDict = keys
            .Zip(cacheList, (k, v) => new { Key = k, Value = v })
            .ToDictionary(x => x.Key, x => (int)x.Value);

        var jobPosts = await dbContext.JobPosts
            .Where(j => jobIds.Contains(j.Id))
            .ToListAsync(cancellationToken);

        foreach (var jobPost in jobPosts)
        {
            var cacheKey = $"{CacheKeys.ApplyClickCount}:{jobPost.Id}";

            if (cacheDict.TryGetValue(cacheKey, out var cacheValue)) jobPost.ApplyCount += cacheValue;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        await _db.KeyDeleteAsync(keys);

        logger.LogInformation("Persisted and cleared apply count for {Count} jobs", keys.Length);
    }

    private async Task<bool> IsAlreadyApplied(string ipAddress, Guid jobId, CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.ApplyClickCount}:{jobId}:{ipAddress}";

        return await _db.KeyExistsAsync(cacheKey);
    }
}
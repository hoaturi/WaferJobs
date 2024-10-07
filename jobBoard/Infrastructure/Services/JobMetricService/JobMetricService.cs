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

    public async Task PersistApplicationCountAsync(CancellationToken cancellationToken)
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

        logger.LogInformation("Persisted {JobCount} temporary job application counts from memory to database",
            jobPosts.Count);
    }

    public async Task IncrementApplicationCountJob(Guid jobId, CancellationToken cancellationToken)
    {
        var ipAddress = httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ipAddress)) return;
        if (await IsAlreadyApplied(ipAddress, jobId)) return;

        var cacheKey = $"{CacheKeys.ApplyClickCount}:{jobId}";
        var cacheKeyWithIp = $"{CacheKeys.ApplyClickCount}:{jobId}:{ipAddress}";

        await _db.StringIncrementAsync(cacheKey);
        await _db.StringSetAsync(cacheKeyWithIp, 1, TimeSpan.FromDays(1));
    }

    private async Task<bool> IsAlreadyApplied(string ipAddress, Guid jobId)
    {
        var cacheKey = $"{CacheKeys.ApplyClickCount}:{jobId}:{ipAddress}";

        return await _db.KeyExistsAsync(cacheKey);
    }
}
using System.Collections.Immutable;
using JobBoard.Common.Interfaces;
using JobBoard.Domain.JobPost;
using JobBoard.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using static MessagePack.MessagePackSerializer;

namespace JobBoard.Infrastructure.Services.LocationService;

public class LocationService(
    IDistributedCache cache,
    AppDbContext dbContext,
    ILogger<LocationService> logger)
    : ILocationService
{
    private const string CountriesCacheKey = "Countries:WithJobPosts";
    private const string CitiesCacheKey = "Cities:WithJobPosts";
    private static readonly TimeSpan CacheExpirationTime = TimeSpan.FromHours(1);
    private static readonly TimeSpan CacheOperationTimeout = TimeSpan.FromSeconds(5);

    public async Task<int?> GetOrCreateCityIdAsync(string? cityName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cityName))
            return null;

        var normalizedCityName = cityName.Trim().ToLowerInvariant();

        var city = await dbContext.Cities
            .FirstOrDefaultAsync(c => c.Label.ToLower() == normalizedCityName, cancellationToken);

        if (city is not null)
            return city.Id;

        var newCity = new CityEntity
        {
            Label = cityName.Trim(),
            Slug = normalizedCityName.Replace(" ", "-")
        };

        dbContext.Cities.Add(newCity);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created new city with ID: {CityId}", newCity.Id);

        await RefreshCitiesCacheAsync(cancellationToken);

        return newCity.Id;
    }

    public async Task<ImmutableArray<CityDto>> GetCitiesWithActiveJobPostAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var serializedCities = await cache.GetAsync(CitiesCacheKey,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            if (serializedCities is null) return await RefreshCitiesCacheAsync(cancellationToken);
            var cities = Deserialize<ImmutableArray<CityDto>>(serializedCities,
                cancellationToken: cancellationToken);

            if (cities.Length <= 0) return await RefreshCitiesCacheAsync(cancellationToken);
            return cities;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Cache operation timed out for cities. Falling back to database.");
            return await GetCitiesFromDatabaseAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting cities.");
            return ImmutableArray<CityDto>.Empty;
        }
    }

    public async Task<ImmutableArray<CountryDto>> GetCountriesWithActiveJobPostAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var serializedCountries = await cache.GetAsync(CountriesCacheKey,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            if (serializedCountries is null) return await RefreshCountriesCacheAsync(cancellationToken);
            var countries = Deserialize<ImmutableArray<CountryDto>>(serializedCountries,
                cancellationToken: cancellationToken);

            if (countries.Length <= 0) return await RefreshCountriesCacheAsync(cancellationToken);
            return countries;
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Cache operation timed out for countries. Falling back to database.");
            return await GetCountriesFromDatabaseAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting countries.");
            return ImmutableArray<CountryDto>.Empty;
        }
    }

    private async Task<ImmutableArray<CountryDto>> RefreshCountriesCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            var countries = await GetCountriesFromDatabaseAsync(cancellationToken);

            if (countries.Length > 0)
            {
                var serializedCountries = Serialize(countries, cancellationToken: cancellationToken);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpirationTime
                };
                await cache.SetAsync(CountriesCacheKey, serializedCountries, options,
                    new CancellationTokenSource(CacheOperationTimeout).Token);

                logger.LogInformation("Updated cache with {Count} countries from database", countries.Length);
            }
            else
            {
                logger.LogWarning("No countries found in database to refresh cache");
            }

            return countries;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh countries cache.");
            return ImmutableArray<CountryDto>.Empty;
        }
    }

    private async Task<ImmutableArray<CityDto>> RefreshCitiesCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cities = await GetCitiesFromDatabaseAsync(cancellationToken);

            if (cities.Length > 0)
            {
                var serializedCities = Serialize(cities, cancellationToken: cancellationToken);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpirationTime
                };
                await cache.SetAsync(CitiesCacheKey, serializedCities, options,
                    new CancellationTokenSource(CacheOperationTimeout).Token);

                logger.LogInformation("Updated cache with {Count} cities from database", cities.Length);
            }
            else
            {
                logger.LogWarning("No cities found in database to refresh cache");
            }

            return cities;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh cities cache.");
            return ImmutableArray<CityDto>.Empty;
        }
    }

    private async Task<ImmutableArray<CountryDto>> GetCountriesFromDatabaseAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching countries from database...");

        var countries = await dbContext.JobPosts.AsNoTracking()
            .Where(j => !j.IsDeleted && j.IsPublished)
            .Select(j => j.Country)
            .Distinct()
            .Select(c => new CountryDto(
                c.Id,
                c.Label,
                c.Slug))
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} countries in the database", countries.Count);

        return [..countries];
    }

    private async Task<ImmutableArray<CityDto>> GetCitiesFromDatabaseAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching cities from database...");

        var cities = await dbContext.JobPosts.AsNoTracking()
            .Where(j => j.City != null && !j.IsDeleted && j.IsPublished)
            .Select(j => j.City)
            .Distinct()
            .Select(c => new CityDto(
                c!.Id,
                c.Label,
                c.Slug))
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} cities in the database", cities.Count);

        return [..cities];
    }
}
using JobBoard.Common.Constants;
using JobBoard.Domain.Common;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.LocationService;

public class LocationService(IDistributedCache cache, AppDbContext dbContext, ILogger<LocationService> logger)
    : ILocationService
{
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

    public async Task<IReadOnlyList<LocationDto>> GetLocationsWithActiveJobPostAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var serializedLocations = await cache.GetAsync(CacheKeys.LocationsCacheKey,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            if (serializedLocations is null || serializedLocations.Length == 0)
                return await RefreshLocationsCacheAsync(cancellationToken);

            var locations =
                MessagePackSerializer.Deserialize<List<LocationDto>>(serializedLocations,
                    cancellationToken: cancellationToken);

            return locations.Count != 0 ? locations : await RefreshLocationsCacheAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Cache operation timed out for locations. Falling back to database.");
            return await GetLocationsFromDatabaseAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting locations.");
            return new List<LocationDto>();
        }
    }

    public async Task<IReadOnlyList<CityDto>> GetCitiesWithActiveJobPostAsync(CancellationToken cancellationToken)
    {
        try
        {
            var serializedCities = await cache.GetAsync(CacheKeys.CitiesCacheKey,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            if (serializedCities is null || serializedCities.Length == 0)
                return await RefreshCitiesCacheAsync(cancellationToken);

            var cities = MessagePackSerializer.Deserialize<List<CityDto>>(serializedCities,
                cancellationToken: cancellationToken);

            return cities.Count != 0 ? cities : await RefreshCitiesCacheAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Cache operation timed out for cities. Falling back to database.");
            return await GetCitiesFromDatabaseAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting cities.");
            return new List<CityDto>();
        }
    }

    public async Task<IReadOnlyList<CountryDto>> GetCountriesWithActiveJobPostAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var serializedCountries = await cache.GetAsync(CacheKeys.CountriesCacheKey,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            if (serializedCountries is null || serializedCountries.Length == 0)
                return await RefreshCountriesCacheAsync(cancellationToken);

            var countries = MessagePackSerializer.Deserialize<List<CountryDto>>(serializedCountries,
                cancellationToken: cancellationToken);

            return countries.Count != 0 ? countries : await RefreshCountriesCacheAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Cache operation timed out for countries. Falling back to database.");
            return await GetCountriesFromDatabaseAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while getting countries.");
            return new List<CountryDto>();
        }
    }

    private async Task<IReadOnlyList<LocationDto>> RefreshLocationsCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            var locations = await GetLocationsFromDatabaseAsync(cancellationToken);

            if (locations.Count != 0)
            {
                var serializedLocations =
                    MessagePackSerializer.Serialize(locations, cancellationToken: cancellationToken);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpirationTime
                };
                await cache.SetAsync(CacheKeys.LocationsCacheKey, serializedLocations, options,
                    new CancellationTokenSource(CacheOperationTimeout).Token);

                logger.LogInformation("Updated cache with {Count} locations from database", locations.Count);
            }
            else
            {
                logger.LogWarning("No locations found in database to refresh cache");
            }

            return locations;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh locations cache.");
            return new List<LocationDto>();
        }
    }

    private async Task<IReadOnlyList<CountryDto>> RefreshCountriesCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            var countries = await GetCountriesFromDatabaseAsync(cancellationToken);

            if (countries.Count != 0)
            {
                var serializedCountries =
                    MessagePackSerializer.Serialize(countries, cancellationToken: cancellationToken);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpirationTime
                };
                await cache.SetAsync(CacheKeys.CountriesCacheKey, serializedCountries, options,
                    new CancellationTokenSource(CacheOperationTimeout).Token);

                logger.LogInformation("Updated cache with {Count} countries from database", countries.Count);
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
            return new List<CountryDto>();
        }
    }

    private async Task<IReadOnlyList<CityDto>> RefreshCitiesCacheAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cities = await GetCitiesFromDatabaseAsync(cancellationToken);

            if (cities.Count != 0)
            {
                var serializedCities = MessagePackSerializer.Serialize(cities, cancellationToken: cancellationToken);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpirationTime
                };
                await cache.SetAsync(CacheKeys.CitiesCacheKey, serializedCities, options,
                    new CancellationTokenSource(CacheOperationTimeout).Token);

                logger.LogInformation("Updated cache with {Count} cities from database", cities.Count);
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
            return new List<CityDto>();
        }
    }

    private async Task<IReadOnlyList<LocationDto>> GetLocationsFromDatabaseAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching distinct locations from database...");

        var locations = await dbContext.JobPosts
            .AsNoTracking()
            .Where(j => !j.IsDeleted && j.IsPublished)
            .Select(j => new
            {
                CountryLabel = j.Country.Label,
                CityLabel = j.City != null ? j.City.Label : null,
                CountrySlug = j.Country.Slug,
                CitySlug = j.City != null ? j.City.Slug : null
            })
            .Distinct()
            .Select(l => new LocationDto(
                l.CityLabel != null ? $"{l.CityLabel}, {l.CountryLabel}" : l.CountryLabel,
                l.CitySlug,
                l.CountrySlug
            ))
            .ToListAsync(cancellationToken);

        logger.LogInformation("Found {Count} distinct locations in the database", locations.Count);

        return locations;
    }

    private async Task<IReadOnlyList<CountryDto>> GetCountriesFromDatabaseAsync(CancellationToken cancellationToken)
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

        return countries;
    }

    private async Task<IReadOnlyList<CityDto>> GetCitiesFromDatabaseAsync(CancellationToken cancellationToken)
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

        return cities;
    }
}
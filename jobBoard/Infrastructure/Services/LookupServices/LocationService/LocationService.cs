using JobBoard.Common.Constants;
using JobBoard.Domain.Common;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.LookupServices.LocationService;

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
        var serializedLocations = await cache.GetAsync(CacheKeys.LocationsCacheKey,
            new CancellationTokenSource(CacheOperationTimeout).Token);

        if (serializedLocations is null || serializedLocations.Length == 0)
            return await RefreshLocationsCacheAsync(cancellationToken);

        var locations = MessagePackSerializer.Deserialize<IReadOnlyList<LocationDto>>(serializedLocations,
            cancellationToken: cancellationToken);

        return locations.Count != 0 ? locations : await RefreshLocationsCacheAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CityDto>> GetCitiesWithActiveJobPostAsync(CancellationToken cancellationToken)
    {
        var serializedCities = await cache.GetAsync(CacheKeys.CitiesCacheKey,
            new CancellationTokenSource(CacheOperationTimeout).Token);

        if (serializedCities is null || serializedCities.Length == 0)
            return await RefreshCitiesCacheAsync(cancellationToken);

        var cities = MessagePackSerializer.Deserialize<IReadOnlyList<CityDto>>(serializedCities,
            cancellationToken: cancellationToken);

        return cities.Count != 0 ? cities : await RefreshCitiesCacheAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CountryDto>> GetCountriesWithActiveJobPostAsync(
        CancellationToken cancellationToken)
    {
        var serializedCountries = await cache.GetAsync(CacheKeys.CountriesCacheKey,
            new CancellationTokenSource(CacheOperationTimeout).Token);

        if (serializedCountries is null || serializedCountries.Length == 0)
            return await RefreshCountriesCacheAsync(cancellationToken);

        var countries = MessagePackSerializer.Deserialize<IReadOnlyList<CountryDto>>(serializedCountries,
            cancellationToken: cancellationToken);

        return countries.Count != 0 ? countries : await RefreshCountriesCacheAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<LocationDto>> RefreshLocationsCacheAsync(CancellationToken cancellationToken)
    {
        var locations = await GetLocationsFromDatabaseAsync(cancellationToken);

        if (locations.Count != 0)
        {
            var serializedLocations = MessagePackSerializer.Serialize(locations, cancellationToken: cancellationToken);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpirationTime
            };
            await cache.SetAsync(CacheKeys.LocationsCacheKey, serializedLocations, options,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            logger.LogInformation("Updated location cache with {Count} items from database", locations.Count);
        }
        else
        {
            logger.LogWarning("No locations found in database to refresh cache");
        }

        return locations;
    }

    private async Task<IReadOnlyList<CountryDto>> RefreshCountriesCacheAsync(CancellationToken cancellationToken)
    {
        var countries = await GetCountriesFromDatabaseAsync(cancellationToken);

        if (countries.Count != 0)
        {
            var serializedCountries = MessagePackSerializer.Serialize(countries, cancellationToken: cancellationToken);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = CacheExpirationTime
            };
            await cache.SetAsync(CacheKeys.CountriesCacheKey, serializedCountries, options,
                new CancellationTokenSource(CacheOperationTimeout).Token);

            logger.LogInformation("Updated countries cache with {Count} items from database", countries.Count);
        }
        else
        {
            logger.LogWarning("No countries found in database to refresh cache");
        }

        return countries;
    }

    private async Task<IReadOnlyList<CityDto>> RefreshCitiesCacheAsync(CancellationToken cancellationToken)
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

            logger.LogInformation("Updated cities cache with {Count} items from database", cities.Count);
        }
        else
        {
            logger.LogWarning("No cities found in database to refresh cache");
        }

        return cities;
    }

    private async Task<IReadOnlyList<LocationDto>> GetLocationsFromDatabaseAsync(CancellationToken cancellationToken)
    {
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


        return locations;
    }

    private async Task<IReadOnlyList<CountryDto>> GetCountriesFromDatabaseAsync(CancellationToken cancellationToken)
    {
        var countries = await dbContext.JobPosts.AsNoTracking()
            .Where(j => !j.IsDeleted && j.IsPublished)
            .Select(j => j.Country)
            .Distinct()
            .Select(c => new CountryDto(
                c.Id,
                c.Label,
                c.Slug))
            .ToListAsync(cancellationToken);


        return countries;
    }

    private async Task<IReadOnlyList<CityDto>> GetCitiesFromDatabaseAsync(CancellationToken cancellationToken)
    {
        var cities = await dbContext.JobPosts.AsNoTracking()
            .Where(j => j.City != null && !j.IsDeleted && j.IsPublished)
            .Select(j => j.City)
            .Distinct()
            .Select(c => new CityDto(
                c!.Id,
                c.Label,
                c.Slug))
            .ToListAsync(cancellationToken);


        return cities;
    }
}
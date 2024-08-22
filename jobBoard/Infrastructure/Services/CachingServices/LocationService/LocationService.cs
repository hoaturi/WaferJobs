using JobBoard.Common.Constants;
using JobBoard.Domain.Common;
using JobBoard.Infrastructure.Persistence;
using MessagePack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace JobBoard.Infrastructure.Services.CachingServices.LocationService;

public class LocationService(IDistributedCache cache, AppDbContext dbContext, ILogger<LocationService> logger)
    : ILocationService
{
    private static readonly TimeSpan CacheOperationTimeout = TimeSpan.FromSeconds(5);

    public async Task<CityEntity?> GetOrCreateCityAsync(string cityName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(cityName)) return null;

        var normalizedCity = cityName.Trim().ToLowerInvariant().Replace(" ", "-");

        var city = await dbContext.Cities
            .FirstOrDefaultAsync(c => c.Slug == normalizedCity, cancellationToken);

        if (city != null) return city;

        city = new CityEntity { Label = cityName, Slug = normalizedCity };
        dbContext.Cities.Add(city);

        return city;
    }

    public async Task<IReadOnlyList<LocationDto>> GetLocationsWithActiveJobPostAsync(
        CancellationToken cancellationToken)
    {
        var serializedLocations = await cache.GetAsync(CacheKeys.Locations,
            new CancellationTokenSource(CacheOperationTimeout).Token);

        if (serializedLocations is null || serializedLocations.Length == 0)
            return await RefreshLocationsCacheAsync(cancellationToken);

        var locations = MessagePackSerializer.Deserialize<IReadOnlyList<LocationDto>>(serializedLocations,
            cancellationToken: cancellationToken);

        return locations.Count != 0 ? locations : await RefreshLocationsCacheAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CityDto>> GetCitiesWithActiveJobPostAsync(CancellationToken cancellationToken)
    {
        var serializedCities = await cache.GetAsync(CacheKeys.Cities,
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
        var serializedCountries = await cache.GetAsync(CacheKeys.Countries,
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

            await cache.SetAsync(CacheKeys.Locations, serializedLocations, cancellationToken);
            logger.LogInformation("Updated location cache with {Count} items", locations.Count);
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

            await cache.SetAsync(CacheKeys.Countries, serializedCountries, cancellationToken);
            logger.LogInformation("Updated countries cache with {Count} items", countries.Count);
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

            await cache.SetAsync(CacheKeys.Cities, serializedCities, cancellationToken);
            logger.LogInformation("Updated cities cache with {Count} items", cities.Count);
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
            .Select(j => new LocationDto(
                j.City != null ? $"{j.City.Label}, {j.Country.Label}" : j.Country.Label,
                j.City != null ? j.City.Slug : null,
                j.Country.Slug,
                j.CityId,
                j.CountryId
            ))
            .Distinct()
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
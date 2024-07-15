using JobBoard.Infrastructure.Services.LocationService;

namespace JobBoard.Common.Interfaces;

public interface ILocationService
{
    Task<IReadOnlyList<CountryDto>> GetCountriesWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<CityDto>> GetCitiesWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<LocationDto>> GetLocationsWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<int?> GetOrCreateCityIdAsync(string? cityName, CancellationToken cancellationToken);
}
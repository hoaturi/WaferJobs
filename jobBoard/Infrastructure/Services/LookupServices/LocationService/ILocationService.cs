using JobBoard.Domain.Common;

namespace JobBoard.Infrastructure.Services.LookupServices.LocationService;

public interface ILocationService
{
    Task<IReadOnlyList<CountryDto>> GetCountriesWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<CityDto>> GetCitiesWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<LocationDto>> GetLocationsWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<CityEntity?> GetOrCreateCityAsync(string city, CancellationToken cancellationToken);
}
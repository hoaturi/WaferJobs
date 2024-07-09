using System.Collections.Immutable;
using JobBoard.Infrastructure.Services.LocationService;

namespace JobBoard.Common.Interfaces;

public interface ILocationService
{
    Task<ImmutableArray<CountryDto>> GetCountriesWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<ImmutableArray<CityDto>> GetCitiesWithActiveJobPostAsync(CancellationToken cancellationToken);

    Task<int?> GetOrCreateCityIdAsync(string? cityName, CancellationToken cancellationToken);
}
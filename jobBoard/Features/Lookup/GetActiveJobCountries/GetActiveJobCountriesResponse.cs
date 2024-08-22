using JobBoard.Infrastructure.Services.CachingServices.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobCountries;

public record GetActiveJobCountriesResponse(IReadOnlyList<CountryDto> Countries);
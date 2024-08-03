using System.Collections.Immutable;
using JobBoard.Infrastructure.Services.LookupServices.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobCountries;

public record GetActiveJobCountriesResponse(IReadOnlyList<CountryDto> Countries);
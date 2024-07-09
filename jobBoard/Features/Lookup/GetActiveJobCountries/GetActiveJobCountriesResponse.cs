using System.Collections.Immutable;
using JobBoard.Infrastructure.Services.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobCountries;

public record GetActiveJobCountriesResponse(ImmutableArray<CountryDto> Countries);
using System.Collections.Immutable;
using JobBoard.Infrastructure.Services.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public record GetActiveJobLocationsResponse(ImmutableArray<CountryDto> Countries, ImmutableArray<CityDto> Cities);
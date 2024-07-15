using JobBoard.Infrastructure.Services.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public record GetActiveJobLocationsResponse(IReadOnlyList<LocationDto> Locations);
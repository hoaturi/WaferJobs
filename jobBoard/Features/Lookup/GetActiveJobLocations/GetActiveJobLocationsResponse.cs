using JobBoard.Infrastructure.Services.LookupServices.LocationService;

namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public record GetActiveJobLocationsResponse(IReadOnlyList<LocationDto> Locations);
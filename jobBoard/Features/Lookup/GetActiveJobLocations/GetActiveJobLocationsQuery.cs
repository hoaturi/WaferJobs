using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetActiveJobLocations;

public class GetActiveJobLocationsQuery : IRequest<Result<GetActiveJobLocationsResponse, Error>>;
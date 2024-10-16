using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Lookup.GetActiveJobLocations;

public class GetActiveJobLocationsQuery : IRequest<Result<GetActiveJobLocationsResponse, Error>>;
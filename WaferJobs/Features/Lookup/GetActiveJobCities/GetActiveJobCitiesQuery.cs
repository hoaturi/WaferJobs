using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Lookup.GetActiveJobCities;

public record GetActiveJobCitiesQuery : IRequest<Result<GetActiveJobCitiesResponse, Error>>;
using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Conference.GetConferences;

public record GetConferencesQuery : IRequest<Result<GetConferencesResponse, Error>>;
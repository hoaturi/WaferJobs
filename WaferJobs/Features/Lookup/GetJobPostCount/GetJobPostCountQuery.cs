using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Lookup.GetJobPostCount;

public record GetJobPostCountQuery : IRequest<Result<GetJobPostCountResponse, Error>>;
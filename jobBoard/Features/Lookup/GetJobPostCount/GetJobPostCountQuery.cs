using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Lookup.GetJobPostCount;

public record GetJobPostCountQuery : IRequest<Result<GetJobPostCountResponse, Error>>;
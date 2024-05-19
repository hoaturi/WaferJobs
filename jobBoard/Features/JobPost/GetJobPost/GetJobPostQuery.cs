using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetJobPost;

public record GetJobPostQuery(Guid Id) : IRequest<Result<GetJobPostResponse, Error>>;
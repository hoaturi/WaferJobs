using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.GetJobPost;

public record GetJobPostQuery(string Slug) : IRequest<Result<GetJobPostResponse, Error>>;
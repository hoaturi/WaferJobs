using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.GetMyJobPost;

public record GetMyJobPostQuery(string Slug) : IRequest<Result<GetMyJobPostResponse, Error>>;
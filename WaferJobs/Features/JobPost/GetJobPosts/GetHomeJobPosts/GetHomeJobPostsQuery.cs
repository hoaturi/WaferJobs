using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.GetJobPosts.GetHomeJobPosts;

public record GetHomeJobPostsQuery : IRequest<Result<GetHomeJobPostsResponse, Error>>;
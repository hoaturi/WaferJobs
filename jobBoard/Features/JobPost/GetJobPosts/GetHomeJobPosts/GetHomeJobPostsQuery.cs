using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetJobPosts.GetHomeJobPosts;

public record GetHomeJobPostsQuery : IRequest<Result<GetHomeJobPostsResponse, Error>>;
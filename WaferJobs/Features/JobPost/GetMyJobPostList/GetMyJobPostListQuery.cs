using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.GetMyJobPostList;

public record GetMyJobPostListQuery(string? Status, int Page) : IRequest<Result<GetMyJobPostListResponse, Error>>;
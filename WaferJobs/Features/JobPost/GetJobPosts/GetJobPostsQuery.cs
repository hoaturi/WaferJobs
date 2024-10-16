using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.JobPost.GetJobPosts;

public record GetJobPostsQuery(
    string? Keyword,
    int? City,
    int? Country,
    List<int>? ExperienceLevels,
    bool? RemoteOnly,
    int? PostedDate,
    List<int>? Categories,
    List<int>? EmploymentTypes,
    int? MinSalary,
    int Take,
    int Page
) : IRequest<Result<GetJobPostsResponse, Error>>;
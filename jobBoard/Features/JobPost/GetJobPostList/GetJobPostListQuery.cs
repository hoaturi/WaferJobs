using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetJobPostList;

public record GetJobPostListQuery(
    string? Keyword,
    int? City,
    int? Country,
    int? ExperienceLevel,
    bool? RemoteOnly,
    int? PostedDate,
    List<int>? Categories,
    List<int>? EmploymentTypes,
    bool? FeaturedOnly,
    int? Currency,
    int? MinSalary,
    int? MaxSalary,
    int Take,
    int Page
) : IRequest<Result<GetJobPostListResponse, Error>>;
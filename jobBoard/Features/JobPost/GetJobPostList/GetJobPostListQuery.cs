using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetJobPostList;

public record GetJobPostListQuery(
    string? Keyword,
    string? City,
    string? Country,
    string? RemoteOnly,
    int? PostedDate,
    List<string>? Categories,
    List<string>? EmploymentTypes,
    string? FeaturedOnly,
    int Take,
    int Page
) : IRequest<Result<GetJobPostListResponse, Error>>;
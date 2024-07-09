using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.GetJobPostList;

public record GetJobPostListQuery(
    string? Keyword,
    string? Location,
    string? Remote,
    List<string>? Categories,
    List<string>? EmploymentTypes,
    int Page
) : IRequest<Result<GetJobPostListResponse, Error>>
{
}
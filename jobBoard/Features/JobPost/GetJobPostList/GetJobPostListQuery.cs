using MediatR;

namespace JobBoard;

public record GetJobPostListQuery(
    string? Keyword,
    string? Country,
    string? Remote,
    List<string>? Categories,
    List<string>? EmploymentTypes,
    int Page
) : IRequest<Result<GetJobPostListResponse, Error>> { }

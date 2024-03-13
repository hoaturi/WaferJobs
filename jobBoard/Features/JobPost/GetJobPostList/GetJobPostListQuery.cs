using MediatR;

namespace JobBoard;

public record GetJobPostListQuery(
    string? Keyword,
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    int Page
) : IRequest<Result<GetJobPostListResponse, Error>> { }

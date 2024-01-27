using MediatR;

namespace JobBoard;

public record CreateJobPostCommand(
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    string Description,
    string Title,
    string CompanyName,
    string ApplyUrl,
    bool IsRemote,
    bool IsFeatured,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency
) : IRequest<Result<CreateJobPostResponse, Error>>;

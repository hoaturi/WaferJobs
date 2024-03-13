using MediatR;

namespace JobBoard;

public record CreateFeaturedJobPostCommand(
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    string Description,
    string Title,
    string CompanyName,
    string ApplyUrl,
    bool IsRemote,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    List<string>? Tags
) : IRequest<Result<CreateFeaturedJobPostResponse, Error>>;

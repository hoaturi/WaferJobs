using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public record UpdateMyJobPostCommand(
    Guid Id,
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    string Description,
    string Title,
    string CompanyName,
    string ApplyUrl,
    bool IsRemote,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    List<string>? Tags
) : IRequest<Result<Unit, Error>>;
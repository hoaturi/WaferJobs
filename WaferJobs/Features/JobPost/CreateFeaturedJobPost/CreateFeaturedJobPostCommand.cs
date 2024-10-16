using MediatR;
using WaferJobs.Common.Models;
using WaferJobs.Features.Payment;

namespace WaferJobs.Features.JobPost.CreateFeaturedJobPost;

public record CreateFeaturedJobPostCommand(
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    int ExperienceLevelId,
    string Description,
    string Title,
    string CompanyName,
    string CompanyEmail,
    string ApplyUrl,
    bool IsRemote,
    string? City,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    int? MinSalary,
    int? MaxSalary,
    int? CurrencyId,
    List<string>? Tags
) : IRequest<Result<CreateJobPostCheckoutSessionResponse, Error>>;
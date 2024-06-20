using JobBoard.Common.Models;
using JobBoard.Features.Payment;
using MediatR;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPost;

public record CreateFeaturedJobPostCommand(
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
    string Description,
    string Title,
    string CompanyName,
    string CompanyEmail,
    string ApplyUrl,
    bool IsRemote,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    List<string>? Tags
) : IRequest<Result<CreateJobPostCheckoutSessionResponse, Error>>;
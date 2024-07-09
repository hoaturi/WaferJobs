using JobBoard.Common.Models;
using JobBoard.Features.Payment;
using MediatR;

namespace JobBoard.Features.JobPost.CreateFeaturedJobPostGuest;

public record CreateFeaturedJobPostGuestCommand(
    int CategoryId,
    int CountryId,
    int EmploymentTypeId,
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
    string? Currency,
    List<string>? Tags,
    BusinessSignupPayload? SignupPayload) : IRequest<Result<CreateJobPostCheckoutSessionResponse, Error>>;

public abstract record BusinessSignupPayload(
    string Name,
    string Email,
    string Password
);
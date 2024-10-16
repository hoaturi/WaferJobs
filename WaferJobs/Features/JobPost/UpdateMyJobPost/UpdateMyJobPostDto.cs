namespace WaferJobs.Features.JobPost.UpdateMyJobPost;

public record UpdateMyJobPostDto(
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
    int? ExperienceLevelId,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    int? CurrencyId,
    List<string>? Tags
);
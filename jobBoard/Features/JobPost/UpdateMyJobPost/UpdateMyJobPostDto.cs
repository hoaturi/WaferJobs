namespace JobBoard.Features.JobPost.UpdateMyJobPost;

public record UpdateMyJobPostDto(
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
);
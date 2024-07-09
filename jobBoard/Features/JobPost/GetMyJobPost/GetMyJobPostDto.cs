namespace JobBoard.Features.JobPost.GetMyJobPost;

public record GetMyJobPostDto(
    Guid Id,
    string Category,
    string Country,
    string EmploymentType,
    string Title,
    string Description,
    bool IsRemote,
    bool IsFeatured,
    string CompanyName,
    string CompanyEmail,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    string ApplyUrl,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    List<string>? Tags,
    Guid? UserId
);
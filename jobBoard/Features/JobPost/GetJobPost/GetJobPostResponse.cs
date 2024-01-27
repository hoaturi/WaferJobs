namespace JobBoard;

public record GetJobPostResponse(
    Guid Id,
    string Category,
    string Country,
    string EmploymentType,
    string Title,
    string Description,
    bool IsRemote,
    bool IsFeatured,
    string CompanyName,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    string? ApplyUrl,
    Guid? BusinessId,
    string? CompanyLogoUrl,
    DateTime PublishedAt
);

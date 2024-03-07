namespace JobBoard;

public record JobPostDto(
    Guid Id,
    string Category,
    string Country,
    string EmploymentType,
    string Title,
    bool IsRemote,
    bool IsFeatured,
    string CompanyName,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    Guid? BusinessId,
    string? CompanyLogoUrl,
    DateTime PublishedAt
);

public record GetJobPostListResponse(List<JobPostDto> JobPostList, int Total);

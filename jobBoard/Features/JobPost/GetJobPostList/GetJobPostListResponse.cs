namespace JobBoard.Features.JobPost.GetJobPostList;

public record GetJobPostListResponse(List<JobPostDto> JobPostList, int Total);

public record JobPostDto(
    Guid Id,
    string Category,
    string Country,
    string EmploymentType,
    string Title,
    string Description,
    bool IsRemote,
    bool IsFeatured,
    string CompanyName,
    string Slug,
    string? ExperienceLevel,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    List<string>? Tags,
    Guid? BusinessId,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    DateTime FeaturedStartDate,
    DateTime FeaturedEndDate,
    DateTime PublishedAt
);
namespace JobBoard.Features.JobPost.GetJobPost;

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
    string Slug,
    string? ExperienceLevel,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    List<string>? Tags,
    string ApplyUrl,
    Guid? BusinessId,
    string? CompanyLogoUrl,
    string? CompanyWebsiteUrl,
    string? CompanySize,
    string? CompanyLocation,
    DateTime FeaturedStartDate,
    DateTime FeaturedEndDate,
    DateTime PublishedAt
);
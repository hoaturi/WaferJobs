namespace JobBoard.Features.JobPost.GetJobPost;

public record GetJobPostResponse(
    JobDetails Job,
    BusinessDetails Business
);

public record JobDetails(
    Guid Id,
    string Category,
    string Country,
    string EmploymentType,
    string Title,
    string Description,
    bool IsRemote,
    bool IsFeatured,
    string Slug,
    string? ExperienceLevel,
    string? City,
    int? MinSalary,
    int? MaxSalary,
    string? Currency,
    List<string>? Tags,
    string ApplyUrl,
    DateTime FeaturedStartDate,
    DateTime FeaturedEndDate,
    DateTime PublishedAt
);

public record BusinessDetails(
    string Name,
    string? Slug,
    string? Description,
    string? LogoUrl,
    string? WebsiteUrl,
    string? Size,
    string? Location
);
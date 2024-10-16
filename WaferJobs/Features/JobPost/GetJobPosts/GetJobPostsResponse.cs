namespace WaferJobs.Features.JobPost.GetJobPosts;

public record GetJobPostsResponse(List<JobPostDto> Jobs, int Total);

public record JobPostDto(
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
    DateTime FeaturedStartDate,
    DateTime FeaturedEndDate,
    DateTime PublishedAt
);

public record BusinessDetails(
    Guid? Id,
    string Name,
    string? LogoUrl,
    string? WebsiteUrl
);
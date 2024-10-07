namespace JobBoard.Infrastructure.BackgroundJobs.JobAlertEmailSenderJob;

public record JobAlertEmailContentDto(
    List<JobAlertEmailJobPostDto> JobPosts,
    int TotalMatchCount
);

public record JobAlertEmailJobPostDto(
    Guid Id,
    string Title,
    string Location,
    string EmploymentType,
    string CompanyName,
    string? CompanyLogoUrl,
    string PublishedAt,
    bool IsRemote,
    bool IsFeatured
);
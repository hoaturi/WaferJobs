namespace JobBoard.Infrastructure.BackgroundJobs.JobAlertSender;

public record JobPostsWithCountDto(
    List<JobPostToSendDto> JobPosts,
    int TotalCount
);

public record JobPostToSendDto(
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
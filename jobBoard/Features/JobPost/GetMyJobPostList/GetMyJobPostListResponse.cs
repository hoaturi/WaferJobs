namespace JobBoard.Features.JobPost.GetMyJobPostList;

public record GetMyJobPostListResponse(List<GetMyJobPostDto> JobPostList, int Total);

public record GetMyJobPostDto(
    Guid Id,
    string Title,
    string Category,
    string EmploymentType,
    string Country,
    string? ExperienceLevel,
    string? City,
    bool IsPublished,
    bool IsFeatured,
    bool RequiresPayment,
    int ApplyCount,
    string Slug,
    DateTime? FeaturedStartDate,
    DateTime? FeaturedEndDate,
    DateTime? PublishedAt,
    DateTime CreatedAt
);
namespace JobBoard.Features.JobPost.GetMyJobPostList;

public record GetMyJobPostListResponse(List<GetMyJobPost> JobPostList, int Total);

public record GetMyJobPost(
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
    DateTime? FeaturedStartDate,
    DateTime? FeaturedEndDate,
    DateTime? PublishedAt,
    DateTime CreatedAt);
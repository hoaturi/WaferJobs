using JobBoard.Domain.JobAlert;

namespace JobBoard.Domain.JobPost;

public class CategoryEntity
{
    public int Id { get; set; }
    public required string Label { get; init; }
    public required string Slug { get; init; }

    public ICollection<JobPostEntity> JobPosts { get; set; } = [];
    public ICollection<JobAlertEntity> JobAlerts { get; set; } = [];
}
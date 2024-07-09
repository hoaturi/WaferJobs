using JobBoard.Common;

namespace JobBoard.Domain.JobPost;

public class CityEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Label { get; init; }
    public required string Slug { get; init; }

    public ICollection<JobPostEntity>? JobPosts { get; set; }
}
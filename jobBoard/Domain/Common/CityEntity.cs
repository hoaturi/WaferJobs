using JobBoard.Common;
using JobBoard.Domain.JobPost;

namespace JobBoard.Domain.Common;

public class CityEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Label { get; init; }
    public required string Slug { get; init; }

    public ICollection<JobPostEntity>? JobPosts { get; set; }
}
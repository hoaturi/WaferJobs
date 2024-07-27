using JobBoard.Common;
using JobBoard.Domain.JobPost;

namespace JobBoard.Domain.Common;

public class TagEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string Slug { get; set; }

    public ICollection<JobPostTagEntity> JobPostTags { get; set; } = null!;
}
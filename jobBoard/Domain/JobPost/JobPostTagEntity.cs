using JobBoard.Common;
using JobBoard.Domain.Common;

namespace JobBoard.Domain.JobPost;

public class JobPostTagEntity : BaseEntity
{
    public Guid JobPostId { get; set; }
    public int TagId { get; set; }

    public JobPostEntity JobPost { get; set; } = null!;
    public TagEntity Tag { get; set; } = null!;
}
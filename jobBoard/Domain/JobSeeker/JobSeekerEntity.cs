using JobBoard.Common;

namespace JobBoard.Domain.JobSeeker;

public class JobSeekerEntity : BaseEntity
{
    public Guid Id { get; init; }

    public required Guid UserId { get; init; }
    public required string Name { get; set; }


    public JobSeekerUserEntity? User { get; init; }
}
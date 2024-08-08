using JobBoard.Common;
using JobBoard.Domain.Auth;

namespace JobBoard.Domain.JobSeeker;

public class JobSeekerEntity : BaseEntity
{
    public Guid Id { get; init; }

    public required Guid UserId { get; init; }
    public required string Name { get; set; }

    public ApplicationUserEntity User { get; init; } = null!;
}
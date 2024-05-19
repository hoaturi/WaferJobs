using JobBoard.Domain.Auth;

namespace JobBoard.Domain.JobSeeker;

public class JobSeekerUserEntity : ApplicationUserEntity
{
    public JobSeekerEntity? JobSeeker { get; set; }
}
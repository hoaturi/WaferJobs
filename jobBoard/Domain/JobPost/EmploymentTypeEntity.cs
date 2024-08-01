using JobBoard.Domain.JobAlert;

namespace JobBoard.Domain.JobPost;

public class EmploymentTypeEntity
{
    public int Id { get; set; }
    public required string Label { get; set; }
    public required string Slug { get; set; }
    public ICollection<JobAlertEntity> JobAlerts { get; set; } = [];
}
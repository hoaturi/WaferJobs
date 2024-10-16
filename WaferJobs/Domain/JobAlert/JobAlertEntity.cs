using WaferJobs.Common;
using WaferJobs.Domain.Common;
using WaferJobs.Domain.JobPost;

namespace WaferJobs.Domain.JobAlert;

public class JobAlertEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? Keyword { get; set; }
    public int? CountryId { get; set; }
    public required string Token { get; set; }
    public DateTime? LastSentAt { get; set; }
    public CountryEntity? Country { get; set; }
    public ICollection<ExperienceLevelEntity> ExperienceLevels { get; set; } = [];
    public ICollection<EmploymentTypeEntity> EmploymentTypes { get; set; } = [];
    public ICollection<CategoryEntity> Categories { get; set; } = [];
}
using JobBoard.Common;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;

namespace JobBoard.Domain.JobAlert;

public class JobAlertEntity : BaseEntity
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? Keyword { get; set; }
    public int? CountryId { get; set; }
    public required string Token { get; set; }
    public DateTime? LastSentAt { get; set; }
    public CountryEntity? Country { get; set; }
    public ICollection<EmploymentTypeEntity> EmploymentTypes { get; set; } = [];
    public ICollection<CategoryEntity> Categories { get; set; } = [];
}
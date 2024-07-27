using JobBoard.Common;
using JobBoard.Domain.Common;
using JobBoard.Domain.JobPost;

namespace JobBoard.Domain.JobAlert;

public class JobAlertEntity : BaseEntity
{
    public int Id { get; set; }
    public required string EmailAddress { get; set; }
    public string? Keyword { get; set; }
    public int? CategoryId { get; set; }
    public int? CountryId { get; set; }
    public int? EmploymentTypeId { get; set; }
    public required string Token { get; set; }

    public DateTime? LastSentAt { get; set; }
    public CategoryEntity? Category { get; set; }
    public CountryEntity? Country { get; set; }
    public EmploymentTypeEntity? EmploymentType { get; set; }
}
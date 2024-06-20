using JobBoard.Common;
using JobBoard.Domain.Business;
using JobBoard.Domain.JobPostEntities;

namespace JobBoard.Domain.JobPost;

public class JobPostEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid? BusinessId { get; set; }
    public int CategoryId { get; set; }
    public int CountryId { get; set; }
    public int EmploymentTypeId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string CompanyName { get; set; }
    public required string ApplyUrl { get; set; }
    public string? City { get; set; }
    public int? MinSalary { get; set; }
    public int? MaxSalary { get; set; }
    public string? Currency { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public string? CompanyWebsiteUrl { get; set; }
    public required bool IsRemote { get; set; }
    public required bool IsFeatured { get; set; }
    public List<string>? Tags { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }
    public bool IsDeleted { get; set; }
    public BusinessEntity? Business { get; set; }
    public CategoryEntity Category { get; set; } = null!;
    public CountryEntity Country { get; set; } = null!;
    public EmploymentTypeEntity EmploymentType { get; set; } = null!;
    public List<JobPostPaymentEntity>? Payments { get; set; }
}
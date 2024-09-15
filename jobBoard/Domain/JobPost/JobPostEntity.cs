using JobBoard.Common;
using JobBoard.Domain.Business;
using JobBoard.Domain.Common;
using NpgsqlTypes;

namespace JobBoard.Domain.JobPost;

public class JobPostEntity : BaseEntity
{
    public Guid Id { get; set; }
    public Guid? BusinessId { get; set; }
    public int CategoryId { get; set; }
    public int? CityId { get; set; }
    public int CountryId { get; set; }
    public int EmploymentTypeId { get; set; }
    public int? ExperienceLevelId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string CompanyName { get; set; }
    public required string CompanyEmail { get; set; }
    public required string ApplyUrl { get; set; }
    public int? MinSalary { get; set; }
    public int? MaxSalary { get; set; }
    public int? CurrencyId { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public string? CompanyWebsiteUrl { get; set; }
    public bool IsRemote { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime? FeaturedStartDate { get; set; }
    public DateTime? FeaturedEndDate { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool IsDeleted { get; set; }
    public int ApplyCount { get; set; }
    public required string Slug { get; set; }
    public BusinessEntity? Business { get; set; }
    public CurrencyEntity? Currency { get; set; }
    public ExperienceLevelEntity? ExperienceLevel { get; set; }
    public CategoryEntity Category { get; set; } = null!;
    public CountryEntity Country { get; set; } = null!;
    public CityEntity? City { get; set; }
    public EmploymentTypeEntity EmploymentType { get; set; } = null!;
    public ICollection<TagEntity> Tags { get; set; } = [];
    public ICollection<JobPostPaymentEntity> Payments { get; set; } = [];
    public NpgsqlTsVector SearchVector { get; set; } = null!;
}
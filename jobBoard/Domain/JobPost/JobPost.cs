namespace JobBoard;

public class JobPost : BaseEntity
{
    public Guid Id { get; set; }
    public Guid? BusinessId { get; set; }
    public required int CategoryId { get; set; }
    public required int CountryId { get; set; }
    public required int EmploymentTypeId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string CompanyName { get; set; }
    public required string ApplyUrl { get; set; }
    public string? City { get; set; }
    public int? MinSalary { get; set; }
    public int? MaxSalary { get; set; }
    public string? Currency { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public required bool IsRemote { get; set; }
    public required bool IsFeatured { get; set; }
    public bool IsPublished { get; set; }
    public DateTime PublishedAt { get; set; }

    public Business? Business { get; set; }
    public Category Category { get; set; } = null!;
    public Country Country { get; set; } = null!;
    public EmploymentType EmploymentType { get; set; } = null!;
    public JobPostPayment? Payment { get; set; }
}

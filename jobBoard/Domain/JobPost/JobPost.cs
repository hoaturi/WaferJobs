using System.ComponentModel.DataAnnotations;

namespace JobBoard;

public class JobPost : BaseEntity
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
    public bool IsRemote { get; set; }
    public bool IsPublished { get; set; }
    public bool IsFeatured { get; set; }

    public Business? Business { get; set; }
    public required Category Category { get; set; }
    public required Country Country { get; set; }
    public required EmploymentType EmploymentType { get; set; }
}

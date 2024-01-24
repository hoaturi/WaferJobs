namespace JobBoard;

public record GetJobPostResponse
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required bool IsRemote { get; init; }
    public required bool IsFeatured { get; init; }
    public Guid? BusinessId { get; init; }
    public string? City { get; init; }
    public int? MinSalary { get; init; }
    public int? MaxSalary { get; init; }
    public string? Currency { get; init; }
    public string? ApplyUrl { get; init; }
    public required string CompanyName { get; init; }
    public string? CompanyLogoUrl { get; init; }
    public DateTime PublishedAt { get; init; }
    public required Category Category { get; init; }
    public required Country Country { get; init; }
    public required EmploymentType EmploymentType { get; init; }
}

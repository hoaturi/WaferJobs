using MediatR;

namespace JobBoard;

public record CreateJobPostCommand : IRequest<Result<CreateJobPostResponse, Error>>
{
    public required int CategoryId { get; init; }
    public required int CountryId { get; init; }
    public required int EmploymentTypeId { get; init; }
    public required string Description { get; init; }
    public required string Title { get; init; }
    public required string CompanyName { get; init; }
    public required string ApplyUrl { get; init; }
    public required bool IsRemote { get; init; }
    public required bool IsFeatured { get; init; }
    public string? City { get; init; }
    public int? MinSalary { get; init; }
    public int? MaxSalary { get; init; }
    public string? Currency { get; init; }
}

namespace JobBoard;

public record GetBusinessResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public string? LogoUrl { get; init; }
    public string? Description { get; init; }
    public string? Location { get; init; }
    public string? Url { get; init; }
    public string? TwitterUrl { get; init; }
    public string? LinkedInUrl { get; init; }
    public BusinessSize? Size { get; init; }
}

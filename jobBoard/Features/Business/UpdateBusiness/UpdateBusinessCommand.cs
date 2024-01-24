using MediatR;

namespace JobBoard;

public record UpdateBusinessCommand : IRequest<Result<Unit, Error>>
{
    public required string Name { get; init; }
    public int? BusinessSizeId { get; init; }
    public string? Description { get; init; }
    public string? Location { get; init; }
    public string? Url { get; init; }
    public string? TwitterUrl { get; init; }
    public string? LinkedInUrl { get; init; }
}

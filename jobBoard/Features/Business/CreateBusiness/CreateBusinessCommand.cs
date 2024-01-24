using MediatR;

namespace JobBoard;

public record CreateBusinessCommand(
    string Name,
    int? Size,
    string? Description,
    string? Location,
    string? Url,
    string? TwitterUrl,
    string? LinkedInUrl
) : IRequest<Result<Unit, Error>>;

using MediatR;

namespace JobBoard;

public record UpdateMyBusinessCommand(
    string Name,
    int? BusinessSizeId,
    string? Description,
    string? Location,
    string? Url,
    string? TwitterUrl,
    string? LinkedInUrl
) : IRequest<Result<Unit, Error>>;

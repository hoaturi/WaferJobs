namespace JobBoard;

public record GetBusinessResponse(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? Description,
    string? Location,
    string? Size,
    string? Url,
    string? TwitterUrl,
    string? LinkedInUrl
);

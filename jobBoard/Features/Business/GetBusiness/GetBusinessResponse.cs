namespace JobBoard.Features.Business.GetBusiness;

public record GetBusinessResponse(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? Description,
    string? Location,
    string? Url,
    string? TwitterUrl,
    string? LinkedInUrl,
    string? Size
);
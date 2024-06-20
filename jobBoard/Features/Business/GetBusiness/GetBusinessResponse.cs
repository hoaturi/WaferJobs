namespace JobBoard.Features.Business.GetBusiness;

public record GetBusinessResponse(
    Guid Id,
    string Name,
    string? LogoUrl,
    string? Description,
    string? Location,
    string? WebsiteUrl,
    string? TwitterUrl,
    string? LinkedinUrl,
    string? Size
);
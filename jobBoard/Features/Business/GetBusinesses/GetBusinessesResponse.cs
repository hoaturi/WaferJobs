namespace JobBoard.Features.Business.GetBusinesses;

public record BusinessListItem(
    Guid Id,
    string Name,
    string? LogoUrl,
    bool IsClaimed
);

public record GetBusinessesResponse(
    List<BusinessListItem> Businesses);
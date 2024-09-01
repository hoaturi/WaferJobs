namespace JobBoard.Features.Admin.Business.GetPendingBusinesses;

public record PendingBusinessDto(
    Guid Id,
    string BusinessName,
    string WebsiteUrl,
    string Domain,
    string Email,
    string Name,
    string Title,
    DateTime CreatedAt
);

public record GetPendingBusinessesQueryResponse(List<PendingBusinessDto> Businesses);
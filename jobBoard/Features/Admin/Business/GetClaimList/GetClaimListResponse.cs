namespace JobBoard.Features.Admin.Business.GetClaimList;

public record ClaimListItem(
    Guid Id,
    string BusinessName,
    string Email,
    string FirstName,
    string LastName,
    string Title,
    DateTime ExpiresAt,
    DateTime CreatedAt
);

public record GetClaimListResponse(
    List<ClaimListItem> Claims
);
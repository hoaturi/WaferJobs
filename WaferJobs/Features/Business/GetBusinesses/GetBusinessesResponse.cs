namespace WaferJobs.Features.Business.GetBusinesses;

public record BusinessListItem(
    Guid Id,
    string Name,
    string Domain,
    string? LogoUrl
);

public record GetBusinessesResponse(
    List<BusinessListItem> Businesses);
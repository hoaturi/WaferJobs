namespace JobBoard;

public static class GetBusinessQueryMapper
{
    public static GetBusinessResponse MapToResponse(Business business) =>
        new(
            Id: business.Id,
            Name: business.Name,
            LogoUrl: business.LogoUrl,
            Description: business.Description,
            Location: business.Location,
            Url: business.Url,
            TwitterUrl: business.TwitterUrl,
            LinkedInUrl: business.LinkedInUrl,
            Size: business.BusinessSize?.Name
        );
}

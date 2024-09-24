namespace JobBoard.Features.Conference.GetConferences;

public record ConferenceItem(
    string Title,
    string Organiser,
    string OrganiserEmail,
    string Location,
    string WebsiteUrl,
    string? LogoUrl,
    DateTime StartDate,
    DateTime EndDate
);

public record GetConferencesResponse(
    List<ConferenceItem> Conferences
);
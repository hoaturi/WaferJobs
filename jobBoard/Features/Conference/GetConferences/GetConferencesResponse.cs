namespace JobBoard.Features.Conference.GetConferences;

public record ConferenceDto(
    string Title,
    string Organiser,
    string OrganiserEmail,
    string Location,
    string WebsiteUrl,
    DateTime StartDate,
    DateTime EndDate
);

public record GetConferencesResponse(
    List<ConferenceDto> Conferences
);
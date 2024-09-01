namespace JobBoard.Features.Admin.Conference.GetPendingConferenceSubmissions;

public record PendingConferenceDto(
    int Id,
    string ContactEmail,
    string ContactName,
    string Title,
    string OrganizerName,
    string OrganizerEmail,
    string Location,
    string WebsiteUrl,
    DateTime StartDate,
    DateTime EndDate
);

public record GetPendingConferencesResponse(
    List<PendingConferenceDto> Conferences
);
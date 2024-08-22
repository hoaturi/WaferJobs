using MessagePack;

namespace JobBoard.Infrastructure.Services.CachingServices.ConferenceService;

[MessagePackObject]
public record ConferenceDto(
    [property: Key(0)] string Title,
    [property: Key(1)] string OrganizerName,
    [property: Key(2)] string OrganizerEmail,
    [property: Key(3)] string Location,
    [property: Key(4)] string WebsiteUrl,
    [property: Key(5)] DateTime StartDate,
    [property: Key(6)] DateTime EndDate
);
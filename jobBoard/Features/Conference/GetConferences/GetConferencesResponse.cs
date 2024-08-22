using JobBoard.Infrastructure.Services.CachingServices.ConferenceService;

namespace JobBoard.Features.Conference.GetConferences;

public record GetConferencesResponse(
    IReadOnlyList<ConferenceDto> Conferences
);
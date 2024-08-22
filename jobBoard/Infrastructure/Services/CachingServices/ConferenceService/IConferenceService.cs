namespace JobBoard.Infrastructure.Services.CachingServices.ConferenceService;

public interface IConferenceService
{
    Task<IReadOnlyList<ConferenceDto>> GetConferencesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ConferenceDto>> RefreshConferencesCacheAsync(CancellationToken cancellationToken);
}
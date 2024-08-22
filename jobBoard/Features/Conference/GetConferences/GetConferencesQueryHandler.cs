using JobBoard.Common.Models;
using JobBoard.Infrastructure.Services.CachingServices.ConferenceService;
using MediatR;

namespace JobBoard.Features.Conference.GetConferences;

public class
    GetConferencesQueryHandler(
        IConferenceService conferenceService
    ) : IRequestHandler<GetConferencesQuery, Result<GetConferencesResponse, Error>>
{
    public async Task<Result<GetConferencesResponse, Error>> Handle(GetConferencesQuery query,
        CancellationToken cancellationToken)
    {
        var conferences = await conferenceService.GetConferencesAsync(cancellationToken);

        return new GetConferencesResponse(conferences);
    }
}
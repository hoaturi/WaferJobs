using JobBoard.Common.Models;
using MediatR;

namespace JobBoard.Features.Admin.Conference.GetPendingConferenceSubmissions;

public class GetPendingConferenceSubmissionsQuery : IRequest<Result<GetPendingConferencesResponse, Error>>;
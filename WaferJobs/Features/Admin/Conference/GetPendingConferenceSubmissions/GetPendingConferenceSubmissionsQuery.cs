using MediatR;
using WaferJobs.Common.Models;

namespace WaferJobs.Features.Admin.Conference.GetPendingConferenceSubmissions;

public class GetPendingConferenceSubmissionsQuery : IRequest<Result<GetPendingConferencesResponse, Error>>;
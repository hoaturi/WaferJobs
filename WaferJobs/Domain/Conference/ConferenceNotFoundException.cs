using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Conference;

public class ConferenceNotFoundException(int conferenceId) : CustomException(
    ErrorCodes.ConferenceNotFound,
    HttpStatusCode.NotFound,
    $"Conference {conferenceId} not found."
);
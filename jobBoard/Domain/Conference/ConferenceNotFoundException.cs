using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Conference;

public class ConferenceNotFoundException(int conferenceId) : CustomException(
    ErrorCodes.ConferenceNotFound,
    HttpStatusCode.NotFound,
    $"Conference {conferenceId} not found."
);
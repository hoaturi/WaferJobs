using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.JobAlert;

public static class JobAlertError
{
    public static readonly Error JobAlertAlreadyExists =
        new(
            ErrorCodes.JobAlertAlreadyExists,
            HttpStatusCode.Conflict,
            "Job alert with this email already exists"
        );

    public static readonly Error JobAlertNotFound =
        new(
            ErrorCodes.JobAlertNotFound,
            HttpStatusCode.NotFound,
            "Job alert not found"
        );
}
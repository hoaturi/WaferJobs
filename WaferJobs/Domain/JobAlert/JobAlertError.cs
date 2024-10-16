using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;

namespace WaferJobs.Domain.JobAlert;

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
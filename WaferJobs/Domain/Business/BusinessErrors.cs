using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;

namespace WaferJobs.Domain.Business;

public static class BusinessErrors
{
    public static readonly Error BusinessNotFound = new(
        ErrorCodes.BusinessNotFound,
        HttpStatusCode.NotFound,
        "Business profile was not found."
    );

    public static readonly Error InvalidClaimToken = new(
        ErrorCodes.InvalidToken,
        HttpStatusCode.BadRequest,
        "Invalid claim token."
    );

    public static readonly Error InvalidCreationToken = new(
        ErrorCodes.InvalidToken,
        HttpStatusCode.BadRequest,
        "Invalid creation token."
    );

    public static readonly Error InvalidInvitationToken = new(
        ErrorCodes.InvalidToken,
        HttpStatusCode.BadRequest,
        "Invalid invitation token."
    );
}
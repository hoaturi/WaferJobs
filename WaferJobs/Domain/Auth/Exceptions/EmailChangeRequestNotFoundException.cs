using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Auth.Exceptions;

public class EmailChangeRequestNotFoundException(Guid userId) : CustomException(
    ErrorCodes.EmailChangeRequestNotFound,
    HttpStatusCode.NotFound,
    $"Email change request for user {userId} not found."
);
using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Exceptions;

namespace WaferJobs.Domain.Auth.Exceptions;

public class UserNotFoundException(Guid userId) : CustomException(
    ErrorCodes.UserNotFound,
    HttpStatusCode.NotFound,
    $"User {userId} not found."
);
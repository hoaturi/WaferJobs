using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth.Exceptions;

public class EmailChangeRequestNotFoundException(Guid userId) : CustomException(
    ErrorCodes.EmailChangeRequestNotFound,
    HttpStatusCode.NotFound,
    $"No valid email change request found for user {userId}"
);
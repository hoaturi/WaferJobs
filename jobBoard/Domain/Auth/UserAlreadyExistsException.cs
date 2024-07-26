using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth;

public class UserAlreadyExistsException(string email) : CustomException(
    ErrorCodes.UserAlreadyExists,
    HttpStatusCode.Conflict,
    $"User with email {email} already exists."
);
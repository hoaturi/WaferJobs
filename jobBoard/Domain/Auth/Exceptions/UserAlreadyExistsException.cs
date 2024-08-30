using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Domain.Auth.Exceptions;

public class UserAlreadyExistsException(string email) : CustomException(
    ErrorCodes.EmailAlreadyInUse,
    HttpStatusCode.Conflict,
    $"User with email {email} already exists."
);
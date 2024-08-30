using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.Auth;

public static class AuthErrors
{
    public static readonly Error EmailAlreadyInUse =
        new(
            ErrorCodes.EmailAlreadyInUse,
            HttpStatusCode.Conflict,
            "User with this email already exists"
        );

    public static readonly Error InvalidCredentials =
        new(ErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized, "Invalid credentials");

    public static readonly Error InvalidRefreshToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid refresh token");

    public static readonly Error InvalidCurrentPassword =
        new(ErrorCodes.InvalidCurrentPassword, HttpStatusCode.BadRequest, "Invalid current password");

    public static readonly Error EmailNotVerified =
        new(ErrorCodes.EmailNotVerified, HttpStatusCode.BadRequest, "Email not verified");

    public static readonly Error InvalidEmailChangePin =
        new(ErrorCodes.InvalidPin, HttpStatusCode.BadRequest, "Invalid pin");

    public static readonly Error EmailChangeNotAllowedForBusinessMembers =
        new(ErrorCodes.EmailChangeNotAllowed, HttpStatusCode.Forbidden,
            "Email change not allowed for users with business membership");
}
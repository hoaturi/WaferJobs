using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.Auth;

public static class AuthErrors
{
    public static readonly Error UserAlreadyExists =
        new(
            ErrorCodes.UserAlreadyExistsError,
            HttpStatusCode.Conflict,
            "User with this email already exists"
        );

    public static readonly Error InvalidCredentials =
        new(ErrorCodes.InvalidCredentialsError, HttpStatusCode.Unauthorized, "Invalid credentials");

    public static readonly Error UserNotFound =
        new(ErrorCodes.UserNotFoundError, HttpStatusCode.NotFound, "User not found");

    public static readonly Error InvalidToken =
        new(ErrorCodes.InvalidTokenError, HttpStatusCode.BadRequest, "Invalid token");

    public static readonly Error InvalidAccessToken =
        new(ErrorCodes.InvalidTokenError, HttpStatusCode.Unauthorized, "Invalid access token");

    public static readonly Error InvalidRefreshToken =
        new(ErrorCodes.InvalidTokenError, HttpStatusCode.Unauthorized, "Invalid refresh token");

    public static readonly Error InvalidCurrentPassword =
        new(ErrorCodes.InvalidCurrentPasswordError, HttpStatusCode.BadRequest, "Invalid current password");
}
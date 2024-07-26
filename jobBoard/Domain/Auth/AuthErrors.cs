using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.Auth;

public static class AuthErrors
{
    public static readonly Error UserAlreadyExists =
        new(
            ErrorCodes.UserAlreadyExists,
            HttpStatusCode.Conflict,
            "User with this email already exists"
        );

    public static readonly Error InvalidCredentials =
        new(ErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized, "Invalid credentials");

    public static readonly Error UserNotFound =
        new(ErrorCodes.UserNotFound, HttpStatusCode.NotFound, "User not found");

    public static readonly Error InvalidToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.BadRequest, "Invalid token");

    public static readonly Error InvalidAccessToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid access token");

    public static readonly Error InvalidRefreshToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid refresh token");

    public static readonly Error InvalidCurrentPassword =
        new(ErrorCodes.InvalidCurrentPassword, HttpStatusCode.BadRequest, "Invalid current password");
}
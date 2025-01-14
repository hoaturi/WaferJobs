﻿using System.Net;
using WaferJobs.Common.Constants;
using WaferJobs.Common.Models;

namespace WaferJobs.Domain.Auth;

public static class AuthErrors
{
    public static readonly Error EmailAlreadyInUse =
        new(
            ErrorCodes.EmailAlreadyInUse,
            HttpStatusCode.Conflict,
            "Email is already in use"
        );

    public static readonly Error InvalidCredentials =
        new(ErrorCodes.InvalidCredentials, HttpStatusCode.Unauthorized, "Invalid credentials");

    public static readonly Error InvalidRefreshToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid refresh token");

    public static readonly Error InvalidCurrentPassword =
        new(ErrorCodes.InvalidCurrentPassword, HttpStatusCode.BadRequest, "Invalid current password");

    public static readonly Error EmailNotVerified =
        new(ErrorCodes.EmailNotVerified, HttpStatusCode.BadRequest, "Email not verified");

    public static readonly Error InvalidChangeEmailPin =
        new(ErrorCodes.InvalidPin, HttpStatusCode.BadRequest, "Invalid pin");

    public static readonly Error EmailChangeNotAllowedForBusinessMembers =
        new(ErrorCodes.EmailChangeNotAllowed, HttpStatusCode.Forbidden,
            "Email change not allowed for users with business membership");

    public static readonly Error EmailNotChanged =
        new(ErrorCodes.EmailNotChanged, HttpStatusCode.BadRequest, "Email not changed");

    public static readonly Error InvalidEmailConfirmationToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.BadRequest, "Invalid email confirmation token");

    public static readonly Error InvalidPasswordResetToken =
        new(ErrorCodes.InvalidToken, HttpStatusCode.BadRequest, "Invalid password reset token");

    public static readonly Error MaxPinAttemptsReached =
        new(ErrorCodes.MaxPinAttemptsReached, HttpStatusCode.BadRequest, "Max pin attempts reached");
}
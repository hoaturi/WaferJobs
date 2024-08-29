using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.Business;

public static class BusinessErrors
{
    public static readonly Error BusinessNotFound = new(
        ErrorCodes.BusinessNotFound,
        HttpStatusCode.NotFound,
        "Business profile was not found."
    );

    public static readonly Error BusinessClaimExpired = new(
        ErrorCodes.BusinessClaimExpired,
        HttpStatusCode.BadRequest,
        "Business claim has expired."
    );

    public static readonly Error SelfInvitationNotAllowed = new(
        ErrorCodes.SelfInvitationNotAllowed,
        HttpStatusCode.BadRequest,
        "You cannot invite yourself to join your business."
    );

    public static readonly Error UserAlreadyBusinessMember = new(
        ErrorCodes.UserAlreadyMember,
        HttpStatusCode.BadRequest,
        "User is already a member of this business."
    );

    public static readonly Error InvalidOrExpiredInvitationToken = new(
        ErrorCodes.InvalidOrExpiredInvitationToken,
        HttpStatusCode.BadRequest,
        "Invalid invitation token."
    );

    public static readonly Error UserCannotBeInvited = new(
        ErrorCodes.UserCannotBeInvited,
        HttpStatusCode.BadRequest,
        "Unable to send invitation."
    );

    public static readonly Error InvitationEmailMismatch = new(
        ErrorCodes.InvitationEmailMismatch,
        HttpStatusCode.BadRequest,
        "Invitation email does not match the user's email."
    );

    public static readonly Error BusinessClaimAlreadyConfirmed = new(
        ErrorCodes.BusinessClaimAlreadyConfirmed,
        HttpStatusCode.BadRequest,
        "Business claim has already been confirmed."
    );
}
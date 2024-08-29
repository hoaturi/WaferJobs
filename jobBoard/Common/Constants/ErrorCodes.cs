namespace JobBoard.Common.Constants;

public class ErrorCodes
{
    // User Errors
    public static readonly ErrorCodes UserAlreadyExists = new("AU001");
    public static readonly ErrorCodes InvalidCredentials = new("AU002");
    public static readonly ErrorCodes UserNotFound = new("AU003");
    public static readonly ErrorCodes InvalidToken = new("AU004");
    public static readonly ErrorCodes InvalidCurrentPassword = new("AU005");
    public static readonly ErrorCodes EmailNotVerified = new("AU006");
    public static readonly ErrorCodes InvalidPin = new("AU007");

    // Business Errors
    public static readonly ErrorCodes BusinessNotFound = new("BU001");
    public static readonly ErrorCodes BusinessNotFoundForUser = new("BU002");
    public static readonly ErrorCodes BusinessClaimAlreadyExists = new("BU003");
    public static readonly ErrorCodes BusinessAlreadyClaimed = new("BU004");
    public static readonly ErrorCodes UserAlreadyMember = new("BU005");
    public static readonly ErrorCodes InvalidOrMissingBusinessClaim = new("BU006");
    public static readonly ErrorCodes BusinessClaimantNotFound = new("BU007");
    public static readonly ErrorCodes BusinessClaimAlreadyVerified = new("BU008");
    public static readonly ErrorCodes BusinessClaimInProgress = new("BU009");
    public static readonly ErrorCodes UserNotBusinessMember = new("BU010");
    public static readonly ErrorCodes InsufficientBusinessPermission = new("BU011");
    public static readonly ErrorCodes InvitationEmailMismatch = new("BU012");
    public static readonly ErrorCodes BusinessClaimExpired = new("BU013");
    public static readonly ErrorCodes SelfInvitationNotAllowed = new("BU014");
    public static readonly ErrorCodes InvalidOrExpiredInvitationToken = new("BU015");
    public static readonly ErrorCodes UserCannotBeInvited = new("BU016");
    public static readonly ErrorCodes InvitationNotFound = new("BU017");
    public static readonly ErrorCodes EmailDomainMismatch = new("BU018");

    // Business Claim Errors
    public static readonly ErrorCodes BusinessClaimNotConfirmed = new("BC001");
    public static readonly ErrorCodes BusinessClaimAlreadyConfirmed = new("BC002");

    // Job Post Errors
    public static readonly ErrorCodes JobPostNotFound = new("JP001");
    public static readonly ErrorCodes JobPostAlreadyDeleted = new("JP002");
    public static readonly ErrorCodes UnauthorizedJobPostAccess = new("JP003");
    public static readonly ErrorCodes JobPostAlreadyPublished = new("JP004");

    // Conference Errors
    public static readonly ErrorCodes ConferenceNotFound = new("CO001");

    // Validation Errors
    public static readonly ErrorCodes ValidationFailed = new("VA001");

    // Email Errors
    public static readonly ErrorCodes EmailSendFailed = new("EM001");

    // Job Alert Errors
    public static readonly ErrorCodes JobAlertAlreadyExists = new("JA001");
    public static readonly ErrorCodes JobAlertNotFound = new("JA002");

    // Internal Server Errors
    public static readonly ErrorCodes InternalServerError = new("IN001");

    private ErrorCodes(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
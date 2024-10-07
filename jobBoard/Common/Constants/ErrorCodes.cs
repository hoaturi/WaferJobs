namespace JobBoard.Common.Constants;

public class ErrorCodes
{
    // Common Errors (CO)
    public static readonly ErrorCodes InvalidPin = new("CO001");
    public static readonly ErrorCodes TooManyVerificationAttempts = new("CO002");
    public static readonly ErrorCodes ValidationFailed = new("CO003");
    public static readonly ErrorCodes InternalServerError = new("CO004");

    // Authentication Errors (AU)
    public static readonly ErrorCodes EmailAlreadyInUse = new("AU001");
    public static readonly ErrorCodes InvalidCredentials = new("AU002");
    public static readonly ErrorCodes UserNotFound = new("AU003");
    public static readonly ErrorCodes InvalidToken = new("AU004");
    public static readonly ErrorCodes InvalidCurrentPassword = new("AU005");
    public static readonly ErrorCodes EmailNotVerified = new("AU006");
    public static readonly ErrorCodes EmailChangeRequestNotFound = new("AU007");
    public static readonly ErrorCodes EmailChangeNotAllowed = new("AU008");
    public static readonly ErrorCodes EmailNotChanged = new("AU009");

    // Business Errors (BU)
    public static readonly ErrorCodes BusinessNotFound = new("BU001");
    public static readonly ErrorCodes BusinessNotFoundForUser = new("BU002");
    public static readonly ErrorCodes UserAlreadyMember = new("BU003");
    public static readonly ErrorCodes EmailDomainMismatch = new("BU004");
    public static readonly ErrorCodes PublicEmailDomainNotAllowed = new("BU005");
    public static readonly ErrorCodes BusinessMembershipNotFound = new("BU006");
    public static readonly ErrorCodes BusinessMemberNotAdmin = new("BU007");

    // Job Post Errors (JP)
    public static readonly ErrorCodes JobPostNotFound = new("JP001");
    public static readonly ErrorCodes JobPostAlreadyDeleted = new("JP002");
    public static readonly ErrorCodes UnauthorizedJobPostAccess = new("JP003");
    public static readonly ErrorCodes JobPostAlreadyPublished = new("JP004");

    // Conference Errors (CF)
    public static readonly ErrorCodes ConferenceNotFound = new("CF001");

    // Email Errors (EM)
    public static readonly ErrorCodes EmailSendFailed = new("EM001");

    // Job Alert Errors (JA)
    public static readonly ErrorCodes JobAlertAlreadyExists = new("JA001");
    public static readonly ErrorCodes JobAlertNotFound = new("JA002");

    private ErrorCodes(string code)
    {
        Code = code;
    }

    public string Code { get; }
}
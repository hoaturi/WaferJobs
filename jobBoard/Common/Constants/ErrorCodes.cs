namespace JobBoard.Common.Constants;

public class ErrorCodes
{
    // User Errors
    public static readonly ErrorCodes UserAlreadyExistsError = new("AU001");
    public static readonly ErrorCodes InvalidCredentialsError = new("AU002");
    public static readonly ErrorCodes UserNotFoundError = new("AU003");
    public static readonly ErrorCodes InvalidTokenError = new("AU004");

    // Business Errors
    public static readonly ErrorCodes BusinessNotFoundError = new("BU001");
    public static readonly ErrorCodes InvalidBusinessSizeError = new("BU002");
    public static readonly ErrorCodes BusinessAlreadyExistsError = new("BU003");
    public static readonly ErrorCodes AssociatedBusinessNotFoundError = new("BU004");

    // Job Post Errors
    public static readonly ErrorCodes JobPostNotFoundError = new("JP001");
    public static readonly ErrorCodes JobPostAlreadyDeletedError = new("JP002");
    public static readonly ErrorCodes UnauthorizedJobPostAccessError = new("JP003");
    public static readonly ErrorCodes JobPostAlreadyPublishedError = new("JP004");


    // Validation Errors
    public static readonly ErrorCodes ValidationFailedError = new("VA001");

    // Email Errors
    public static readonly ErrorCodes EmailSendFailedError = new("EM001");

    // Payment Errors
    public static readonly ErrorCodes CreateSessionFailedError = new("PA001");
    public static readonly ErrorCodes CreateStripeCustomerFailedError = new("PA002");

    // File Upload Errors
    public static readonly ErrorCodes FileUploadFailedError = new("FI001");

    // Internal Server Errors
    public static readonly ErrorCodes InternalServerError = new("IN001");

    private ErrorCodes(string code)
    {
        Code = code;
    }

    private string Code { get; }

    public override string ToString()
    {
        return Code;
    }
}
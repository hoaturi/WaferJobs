namespace JobBoard;

public readonly struct ErrorCodes
{
    private readonly string _code;

    private ErrorCodes(string code) => _code = code;

    public override string ToString() => _code;

    // User Errors
    public static readonly ErrorCodes UserAlreadyExists = new("AU001");
    public static readonly ErrorCodes InvalidCredentials = new("AU002");
    public static readonly ErrorCodes UserNotFound = new("AU003");
    public static readonly ErrorCodes InvalidToken = new("AU004");

    // Business Errors
    public static readonly ErrorCodes BusinessNotFound = new("BU001");
    public static readonly ErrorCodes InvalidBusinessSize = new("BU002");
    public static readonly ErrorCodes BusinessAlreadyExists = new("BU003");
    public static readonly ErrorCodes AssociatedBusinessNotFound = new("BU004");

    // Job Post Errors
    public static readonly ErrorCodes JobPostNotFound = new("JP001");

    // Validation Errors
    public static readonly ErrorCodes ValidationFailed = new("VA001");

    // Email Errors
    public static readonly ErrorCodes EmailSendFailed = new("EM001");

    // Payment Errors
    public static readonly ErrorCodes CreateSessionFailed = new("PA001");
    public static readonly ErrorCodes CreateCustomerFailed = new("PA002");

    // Internal Server Errors
    public static readonly ErrorCodes InternalServerError = new("IN001");
}

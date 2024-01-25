using System.Net;

namespace JobBoard;

public class CustomException : Exception
{
    public ErrorCodes ErrorCode { get; }
    public HttpStatusCode StatusCode { get; }
    public List<ValidationError>? FieldErrors { get; }

    public CustomException(ErrorCodes errorCode, HttpStatusCode httpStatusCode, string message)
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = httpStatusCode;
    }

    public CustomException(
        ErrorCodes errorCode,
        HttpStatusCode httpStatusCode,
        string message,
        List<ValidationError> fieldErrors
    )
        : base(message)
    {
        ErrorCode = errorCode;
        StatusCode = httpStatusCode;
        FieldErrors = fieldErrors;
    }
};

using System.Net;

namespace JobBoard;

public class CustomException(
    ErrorCodes errorCode,
    HttpStatusCode httpStatusCode,
    string message,
    Dictionary<string, string[]>? fieldErrors = null
) : Exception(message)
{
    public ErrorCodes ErrorCode { get; } = errorCode;
    public HttpStatusCode StatusCode { get; } = httpStatusCode;

    public Dictionary<string, string[]> FieldErrors { get; } = fieldErrors ?? [];
}

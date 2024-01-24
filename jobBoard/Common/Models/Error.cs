using System.Net;

namespace JobBoard;

public class Error(ErrorCodes errorCode, HttpStatusCode statusCode, string message)
{
    public string ErrorCode { get; } = errorCode.ToString();
    public int StatusCode { get; } = (int)statusCode;
    public string Message { get; } = message;
}

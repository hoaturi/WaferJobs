using System.Net;
using JobBoard.Common.Constants;

namespace JobBoard.Common.Exceptions;

public class CustomException(
    ErrorCodes errorCode,
    HttpStatusCode httpStatusCode,
    string message)
    : Exception(message)
{
    public ErrorCodes ErrorCode { get; } = errorCode;
    public HttpStatusCode StatusCode { get; } = httpStatusCode;
}
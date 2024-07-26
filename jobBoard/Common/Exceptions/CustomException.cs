using System.Net;
using JobBoard.Common.Constants;

namespace JobBoard.Common.Exceptions;

public class CustomException(
    ErrorCodes errorCode,
    HttpStatusCode httpStatusCode,
    string message)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode.Code;
    public int StatusCode { get; } = (int)httpStatusCode;
}
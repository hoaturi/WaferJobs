using System.Net;
using JobBoard.Common.Constants;

namespace JobBoard.Common.Models;

public class Error(ErrorCodes errorCode, HttpStatusCode statusCode, string message)
{
    public string ErrorCode { get; } = errorCode.Code;
    public int StatusCode { get; } = (int)statusCode;

    public string Message { get; } = message;
}
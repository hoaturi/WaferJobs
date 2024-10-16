using System.Net;
using WaferJobs.Common.Constants;

namespace WaferJobs.Common.Exceptions;

public class CustomException(
    ErrorCodes errorCode,
    HttpStatusCode httpStatusCode,
    string message)
    : Exception(message)
{
    public string ErrorCode { get; } = errorCode.Code;
    public int StatusCode { get; } = (int)httpStatusCode;
}
using System.Net;
using WaferJobs.Common.Constants;

namespace WaferJobs.Common.Exceptions;

public class InvalidJwtException()
    : CustomException(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid jwt token");
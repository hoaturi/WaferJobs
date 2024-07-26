using System.Net;
using JobBoard.Common.Constants;

namespace JobBoard.Common.Exceptions;

public class InvalidJwtException()
    : CustomException(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid jwt token");
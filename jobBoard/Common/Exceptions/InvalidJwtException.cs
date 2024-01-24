using System.Net;

namespace JobBoard;

public class InvalidJwtException()
    : CustomException(ErrorCodes.InvalidToken, HttpStatusCode.Unauthorized, "Invalid jwt token");

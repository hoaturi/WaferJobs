using System.Net;

namespace JobBoard;

public class UserNotFoundException()
    : CustomException(ErrorCodes.UserNotFound, HttpStatusCode.NotFound, "User not found");

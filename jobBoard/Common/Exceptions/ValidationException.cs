using System.Collections.Generic;
using System.Net;

namespace JobBoard
{
    public class ValidationException(Dictionary<string, string[]> fieldErrors)
        : CustomException(
            ErrorCodes.ValidationFailed,
            HttpStatusCode.BadRequest,
            "Validation failed",
            fieldErrors
        );
}

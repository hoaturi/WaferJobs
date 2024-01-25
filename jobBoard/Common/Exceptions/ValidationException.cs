using System.Collections.Generic;
using System.Net;

namespace JobBoard
{
    public class ValidationException(List<ValidationError> fieldErrors)
        : CustomException(
            ErrorCodes.ValidationFailed,
            HttpStatusCode.BadRequest,
            "Validation failed",
            fieldErrors
        );
}

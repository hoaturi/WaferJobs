using System.Net;

namespace JobBoard;

public class CreateCustomerFailedException()
    : CustomException(
        ErrorCodes.CreateCustomerFailed,
        HttpStatusCode.InternalServerError,
        "Failed to create customer."
    );

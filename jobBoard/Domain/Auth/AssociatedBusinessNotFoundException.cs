using System.Net;

namespace JobBoard;

public class AssociatedBusinessNotFoundException(Guid userId)
    : CustomException(
        ErrorCodes.AssociatedBusinessNotFound,
        HttpStatusCode.NotFound,
        $"User with id: {userId} does not have a business associated with it."
    );

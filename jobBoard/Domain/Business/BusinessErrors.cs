using System.Net;

namespace JobBoard;

public class BusinessErrors
{
    public static Error BusinessNotFound(Guid id) =>
        new(
            ErrorCodes.BusinessNotFound,
            HttpStatusCode.NotFound,
            $"Business profile with id:{id} was not found."
        );

    public static Error BusinessAlreadyExists(Guid userId) =>
        new(
            ErrorCodes.BusinessAlreadyExists,
            HttpStatusCode.Conflict,
            $"User with id: {userId} already has a associated business profile."
        );

    public static Error AssociatedBusinessNotFound(Guid userId) =>
        new(
            ErrorCodes.AssociatedBusinessNotFound,
            HttpStatusCode.BadRequest,
            $"User with id: {userId} does not have a business associated with it."
        );
}

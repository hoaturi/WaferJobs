using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.Business;

public static class BusinessErrors
{
    public static Error BusinessNotFound(Guid id)
    {
        return new Error(
            ErrorCodes.BusinessNotFoundError,
            HttpStatusCode.NotFound,
            $"Business profile with id:{id} was not found."
        );
    }
}
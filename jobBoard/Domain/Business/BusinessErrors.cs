using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Domain.Business;

public static class BusinessErrors
{
    public static readonly Error BusinessNotFound = new(
        ErrorCodes.BusinessNotFound,
        HttpStatusCode.NotFound,
        "Business profile was not found."
    );
}
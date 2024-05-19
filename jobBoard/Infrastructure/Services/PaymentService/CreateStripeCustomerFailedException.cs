using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

namespace JobBoard.Infrastructure.Services.PaymentService;

public class CreateStripeCustomerFailedException()
    : CustomException(
        ErrorCodes.CreateStripeCustomerFailedError,
        HttpStatusCode.InternalServerError,
        "Failed to create customer."
    );
using System.Net;
using JobBoard.Common.Constants;

namespace JobBoard.Common.Exceptions;

public class CustomValidationException(List<ValidationError> errors) : CustomException(
    ErrorCodes.ValidationFailedError,
    HttpStatusCode.BadRequest,
    "Validation failed")
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors;
}

public record ValidationError(string Field, string Message);
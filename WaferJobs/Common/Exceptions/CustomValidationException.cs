using System.Net;
using WaferJobs.Common.Constants;

namespace WaferJobs.Common.Exceptions;

public class CustomValidationException(List<ValidationError> errors) : CustomException(
    ErrorCodes.ValidationFailed,
    HttpStatusCode.BadRequest,
    "Validation failed")
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors;
}

public record ValidationError(string Field, string Message);
using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Models;

namespace JobBoard.Common.Exceptions;

public class ValidationException(List<ValidationError> fieldErrors) : CustomException(
    ErrorCodes.ValidationFailedError,
    HttpStatusCode.BadRequest,
    "Validation failed")
{
    public List<ValidationError> FieldErrors { get; } = fieldErrors;
}
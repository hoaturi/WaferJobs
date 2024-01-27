using System.Net;

namespace JobBoard;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response =
            exception is ValidationException
                ? CreateValidationErrorResponse(exception)
                : CreateErrorResponse(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static ValidationErrorResponse CreateValidationErrorResponse(Exception exception)
    {
        var fieldErrors = GetFieldErrors(exception);
        var statusCode = GetStatusCode(exception);
        var errorCode = GetErrorCode(exception);
        var message = exception.Message;

        return new ValidationErrorResponse(statusCode, errorCode, message, fieldErrors!);
    }

    private static ErrorResponse CreateErrorResponse(Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var errorCode = GetErrorCode(exception);
        var message = exception.Message;

        return new ErrorResponse(statusCode, errorCode, message);
    }

    private static List<ValidationError>? GetFieldErrors(Exception exception) =>
        exception is ValidationException validationException
            ? validationException.FieldErrors
            : default;

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            CustomException customException => (int)customException.StatusCode,
            _ => (int)HttpStatusCode.InternalServerError
        };

    private static string GetErrorCode(Exception exception) =>
        exception switch
        {
            ValidationException _ => ErrorCodes.ValidationFailed.ToString(),
            CustomException customException => customException.ErrorCode.ToString(),
            _ => ErrorCodes.InternalServerError.ToString()
        };
}

public record ValidationErrorResponse(
    int StatusCode,
    string ErrorCode,
    string Message,
    List<ValidationError> FieldErrors
) : ErrorResponse(StatusCode, ErrorCode, Message);

public record ErrorResponse(int StatusCode, string ErrorCode, string Message);

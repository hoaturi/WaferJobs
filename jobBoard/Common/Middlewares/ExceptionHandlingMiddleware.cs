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
        catch (ValidationException ex)
        {
            _logger.LogError(ex, "Validation exception occurred: {Message}", ex.Message);
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (CustomException ex)
        {
            _logger.LogError(ex, "Custom exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = CreateErrorResponse(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static async Task HandleValidationExceptionAsync(
        HttpContext context,
        ValidationException exception
    )
    {
        var response = CreateValidationErrorResponse(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static ValidationErrorResponse CreateValidationErrorResponse(
        ValidationException exception
    )
    {
        var fieldErrors = GetFieldErrors(exception);
        var statusCode = GetStatusCode(exception);
        var errorCode = GetErrorCode(exception);
        var message = exception.Message;

        return new ValidationErrorResponse(statusCode, errorCode, message, fieldErrors);
    }

    private static ErrorResponse CreateErrorResponse(Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var errorCode = GetErrorCode(exception);
        var message = exception.Message;

        return new ErrorResponse(statusCode, errorCode, message);
    }

    private static List<ValidationError> GetFieldErrors(ValidationException exception)
    {
        return exception.FieldErrors!;
    }

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

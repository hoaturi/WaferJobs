using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;
using JobBoard.Common.Models;

namespace JobBoard.Common.Middlewares;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = exception switch
        {
            ValidationException validationException => CreateValidationErrorResponse(validationException),
            CustomException customException => CreateErrorResponse(customException),
            _ => CreateErrorResponse(exception)
        };

        logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
        await WriteResponseAsync(context, response);
    }

    private static async Task WriteResponseAsync(HttpContext context, ErrorResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static ValidationErrorResponse CreateValidationErrorResponse(ValidationException exception)
    {
        var statusCode = DetermineStatusCode(exception);
        var errorCode = DetermineErrorCode(exception);
        var message = exception.Message;
        var fieldErrors = exception.FieldErrors;

        return new ValidationErrorResponse(statusCode, errorCode, message, fieldErrors);
    }

    private static ErrorResponse CreateErrorResponse(Exception exception)
    {
        var statusCode = DetermineStatusCode(exception);
        var errorCode = DetermineErrorCode(exception);
        var message = exception.Message;

        return new ErrorResponse(statusCode, errorCode, message);
    }

    private static int DetermineStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            CustomException customException => (int)customException.StatusCode,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    private static string DetermineErrorCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => nameof(ErrorCodes.ValidationFailedError),
            CustomException customException => nameof(customException.ErrorCode),
            _ => nameof(ErrorCodes.InternalServerError)
        };
    }
}

public record ValidationErrorResponse(
    int StatusCode,
    string ErrorCode,
    string Message,
    List<ValidationError> FieldErrors)
    : ErrorResponse(StatusCode, ErrorCode, Message);

public record ErrorResponse(int StatusCode, string ErrorCode, string Message);
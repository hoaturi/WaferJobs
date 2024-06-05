using System.Net;
using JobBoard.Common.Constants;
using JobBoard.Common.Exceptions;

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
            var response = ex switch
            {
                CustomValidationException validationException => CreateCustomValidationErrorResponse(
                    validationException),
                CustomException customException => CreateCustomErrorResponse(customException),
                _ => CreateInternalServerErrorResponse(ex)
            };

            logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            await WriteResponseAsync(context, response);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, ErrorResponse response)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)response.StatusCode;


        if (response is ValidationErrorResponse validationErrorResponse)
            await context.Response.WriteAsJsonAsync(new
            {
                response.StatusCode,
                response.ErrorCode,
                response.Message,
                validationErrorResponse.Errors
            });
        else
            await context.Response.WriteAsJsonAsync(response);
    }


    private static ValidationErrorResponse CreateCustomValidationErrorResponse(CustomValidationException exception)
    {
        var statusCode = HttpStatusCode.BadRequest;
        var errorCode = ErrorCodes.ValidationFailedError;
        var message = exception.Message;
        var fieldErrors = exception.Errors;

        return new ValidationErrorResponse(statusCode, errorCode, message, fieldErrors);
    }

    private static ErrorResponse CreateCustomErrorResponse(CustomException exception)
    {
        var statusCode = exception.StatusCode;
        var errorCode = exception.ErrorCode;
        var message = exception.Message;

        return new ErrorResponse(statusCode, errorCode, message);
    }


    private static ErrorResponse CreateInternalServerErrorResponse(Exception exception)
    {
        const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        var errorCode = ErrorCodes.InternalServerError;
        const string message = "An unexpected error occurred.";

        return new ErrorResponse(statusCode, errorCode, message);
    }
}

public record ErrorResponse(HttpStatusCode StatusCode, string ErrorCode, string Message)
{
    public ErrorResponse(HttpStatusCode statusCode, ErrorCodes errorCode, string message)
        : this(statusCode, errorCode.ToString(), message)
    {
    }
}

public record ValidationErrorResponse(
    HttpStatusCode StatusCode,
    string ErrorCode,
    string Message,
    IReadOnlyList<ValidationError>? Errors) : ErrorResponse(StatusCode, ErrorCode, Message)
{
    public ValidationErrorResponse(HttpStatusCode statusCode, ErrorCodes errorCode, string message,
        IReadOnlyList<ValidationError>? errors)
        : this(statusCode, errorCode.ToString(), message, errors)
    {
    }
}
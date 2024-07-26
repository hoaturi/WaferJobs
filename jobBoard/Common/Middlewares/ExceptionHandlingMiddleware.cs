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
        context.Response.StatusCode = response.StatusCode;

        if (response is ValidationErrorResponse validationErrorResponse)
            await context.Response.WriteAsJsonAsync(new
            {
                response.StatusCode,
                response.ErrorCode,
                response.Message,
                validationErrorResponse.Errors
            });
        else
            await context.Response.WriteAsJsonAsync(new
            {
                response.StatusCode,
                response.ErrorCode
            });
    }

    private static ValidationErrorResponse CreateCustomValidationErrorResponse(CustomValidationException exception)
    {
        var errorCode = ErrorCodes.ValidationFailed;
        var message = exception.Message;
        var fieldErrors = exception.Errors;

        return new ValidationErrorResponse((int)HttpStatusCode.BadRequest, errorCode.Code, message, fieldErrors);
    }

    private static ErrorResponse CreateCustomErrorResponse(CustomException exception)
    {
        var statusCode = exception.StatusCode;
        var errorCode = exception.ErrorCode;
        var message = string.Empty;

        return new ErrorResponse(statusCode, errorCode, message);
    }

    private static ErrorResponse CreateInternalServerErrorResponse(Exception exception)
    {
        var errorCode = ErrorCodes.InternalServerError;
        var message = string.Empty;

        return new ErrorResponse((int)HttpStatusCode.InternalServerError, errorCode.Code, message);
    }
}

public record ErrorResponse(int StatusCode, string ErrorCode, string Message);

public record ValidationErrorResponse(
    int StatusCode,
    string ErrorCode,
    string Message,
    IReadOnlyList<ValidationError>? Errors) : ErrorResponse(StatusCode, ErrorCode, Message);
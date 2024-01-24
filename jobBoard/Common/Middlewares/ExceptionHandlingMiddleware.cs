using System.Net;

namespace JobBoard;

public class ExceptionHandlingMiddleware : IMiddleware
{
    public ExceptionHandlingMiddleware() { }

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

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        if (exception is not ValidationException)
        {
            Console.WriteLine(exception);
        }

        var response = CreateErrorResponse(exception);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);
    }

    private static ErrorResponse CreateErrorResponse(Exception exception)
    {
        return new ErrorResponse
        {
            StatusCode = GetStatusCode(exception),
            ErrorCode = GetErrorCode(exception),
            Detail = exception.Message,
            FieldErrors = GetFieldErrors(exception)
        };
    }

    private static Dictionary<string, string[]> GetFieldErrors(Exception exception) =>
        exception is ValidationException validationException ? validationException.FieldErrors : [];

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

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string ErrorCode { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public Dictionary<string, string[]> FieldErrors { get; set; } = [];
}

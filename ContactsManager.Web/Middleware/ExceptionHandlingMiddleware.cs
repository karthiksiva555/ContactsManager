using System.Net;

namespace ContactsManager.Web.Middleware;

public class ExceptionHandlingMiddleware (RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            // Log the error message.
            var exception = ex.InnerException ?? ex;
            logger.LogError("{exception} - {message}", exception, exception.Message);
            
            // Commented to let UseExceptionHandler middleware to handle the error
            // context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            // // Give a friendly message to the user.
            // await context.Response.WriteAsync("An unhandled exception occurred. Try again later.");

            throw;
        }
    }    
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static void UseCustomExceptionHandling(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
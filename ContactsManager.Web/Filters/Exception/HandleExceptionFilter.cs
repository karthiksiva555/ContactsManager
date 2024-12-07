using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Exception;

public class HandleExceptionFilter(ILogger<HandleExceptionFilter> logger): IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        logger.LogError(context.Exception, "An error occurred while processing the request.");

        // Create a custom error response
        var errorResponse = new
        {
            Message = "An unexpected error occurred. Please try again later.",
            Details = context.Exception.Message // Optional: Include exception details (useful for debugging)
        };

        // Set the result with custom error response and status code
        context.Result = new ObjectResult(errorResponse)
        {
            StatusCode = 500 // Internal Server Error
        };

        // Mark the exception as handled
        context.ExceptionHandled = true;
        
        return Task.CompletedTask;
    }
}
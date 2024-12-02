using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactsManager.Web.Filters.Action;

public class LogActionAsync(ILogger<LogActionAsync> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var controllerName = context.RouteData.Values["controller"]?.ToString();
        var actionName = context.RouteData.Values["action"]?.ToString();
        
        // Equivalent to OnActionExecuting
        logger.LogInformation("Executing {actionName} in controller {controllerName}", actionName, controllerName);
        
        await next();
        
        // Equivalent to OnActionExecuted
        logger.LogInformation("Executed {actionName} in controller {controllerName}", actionName, controllerName);
    }
}